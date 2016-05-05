/**
* jQuery FrameUI 1.2.2
* 
* 作者：张立鑫
* 日期：2014.08.08
* 说明：UI核心库，在这里初始化创建 FrameUI 对象。 
*       使用 FrameUI 控件库必须引入 jquery-1.9.1.min.js, jquery.easing.js 和 json2.js。
*/
(function ($) {
    // Frame 继承拓展方法
    // @param [parent]：当前控件对象要继承的父类对象，一般为function类型，否则返回自身。
    // @param [settings]：为当前控件继续拓展的属性或方法。
    Function.prototype.Extension = function (parent, settings) {
        if (typeof parent !== "function") {
            return this;
        }

        //保存对父类的引用
        this.Parent = parent.prototype;
        this.Parent.constructor = parent;

        //继承
        var F = function () { };
        F.prototype = parent.prototype;
        this.prototype = new F();
        this.prototype.constructor = this;

        //拓展属性或方法
        if (settings) {
            $.extend(this.prototype, settings);
        }
    };

    // 延时加载执行
    // @param [o]:将代替Function类里this对象类函数执行内容的对象。
    // @param [defer]:指示从当前起多少毫秒后执行。
    // @param [args]:类型是数组，作为参数传给Function类里的this对象类函数。
    Function.prototype.Defer = function (o, defer, args) {
        var fn = this; // 这里的this是一个function函数
        return setTimeout(function () { fn.apply(o, args || []); }, defer);
    };

    // 解析器对象
    $.Parser = {
        Auto: true,
        Elements: ["button", "resizable", "draggable", "bar", "pagination", "tabs", "panel", "grid", "checkbox", "layout", "radio", "accordion", "combobox", "textbox", "spinner", "datebox", "navigation", "navbar", "sidebar", "label", "appbutton", "notify"],
        Do: function (o) {
            for (var index = 0, count = $.Parser.Elements.length; index < count; index++) {
                var control = $.Parser.Elements[index];
                var labels = $(".frame-" + control, o);

                if (labels.length) {
                    var type = "";
                    labels.each(function (i) {
                        var options = {};
                        type = $.Parser._Resolve(control);

                        try {
                            options = $.Parser._BuildOptions(labels[i], control);
                            switch (control) {
                                case "resizable":
                                case "draggable":
                                    $.Frame.Do.call($(labels[i]), type, [], options);
                                    break;
                                default:
                                    $.Frame.Do.call($(labels[i]), type, [options]);
                                    break;
                            }
                        }
                        catch (e) { }
                    });
                }
            }
        },
        _Resolve: function (name) {
            var type = name;
            switch (name) {
                case "checkbox":
                    type = "CheckBox";
                    break;
                case "combobox":
                    type = "ComboBox";
                    break;
                case "textbox":
                    type = "TextBox";
                    break;
                case "datebox":
                    type = "DateBox";
                    break;
                case "navbar":
                    type = "NavBar";
                    break;
                case "sidebar":
                    type = "SideBar";
                    break;
                case "appbutton":
                    type = "AppButton";
                    break;
                default:
                    type = name.substring(0, 1).toUpperCase() + name.substring(1);
                    break;
            }

            return type;
        },
        _BuildOptions: function (dom, type) {
            var attroptions = $(dom).attr("frame-options");
            var options = {};
            switch (type) {
                case "resizable":
                    options = {
                        IdAttr: "FrameResizableId",
                        HasElement: false,
                        PropertyToElement: "Target"
                    };
                    break;
                case "draggable":
                    options = {
                        IdAttr: "FrameDraggableId",
                        HasElement: false,
                        PropertyToElement: "Target"
                    };
                    break;
            }
            if (attroptions) {
                if (attroptions.indexOf("{") != 0)
                    attroptions = "{" + attroptions + "}";
            }

            options = $.extend(options, (new Function("return " + attroptions))() || {});

            return options;
        }
    };

    // 核心对象,所有FrameUI控件都进行调用
    $.Frame = {
        Version: "1.2.2",  // 版本号
        Name: "frame",     // 核心框架对象名称
        ComponentCount: 0, // 组件管理对象池中对象的数量
        Components: {}, // 组件管理对象池
        PlugIns: {},
        Defaults: {},
        Methods: {},
        Controls: {},
        // 错误信息对象
        Error: { IsExist: "组件管理器ID已经存在." },
        // 动态生成组件ID并返回该ID
        // @param [name]:生成组件ID的前缀名称，FrameUI 插件统一由每个插件对象中的_IdPrev()函数提供
        CreateId: function (name) {
            name = name || this.Name;
            var id = name + (10000 + this.ComponentCount);
            this.ComponentCount++;
            return id;
        },
        // 将FrameUI控件添加到组件管理器中
        // @param [component]: FrameUI 插件对象。
        Add: function (component) {
            // 检测控件ID是否已经创建
            if (!component.Id) {
                // 注意:ID未创建
                component.Id = this.CreateId(component._IdPrev());
            }

            // 组件管理器中已存在指定ID的FrameUI控件，抛出异常
            if (this.Components[component.Id])
            //throw new Error(this.Error.IsExist);
                return this.Components[component.Id]

            this.Components[component.Id] = component;
        },
        // 从组件管理对象池中移除指定FrameUI插件对象
        // @param [args]: 这里可以是FrameUI 插件对象或者FrameUI 插件对象的Id
        Remove: function (args) {
            if (typeof args == "string" || typeof args == "number") {
                delete this.Components[args];
            }
            else if (typeof args == "object" && args instanceof $.Frame.Component) {
                delete this.Components[args.Id];
            }
        },
        // 从组件管理器中获取指定FrameUI控件对象
        // @param [args]：FrameUI控件Id 或 Dom Object Array(jQuery)。
        // @param [id]：FrameUI控件对象的属性名称或附加在jQuery Dom 元素对象上的属性名称。
        Get: function (args, id) {
            id = id || this.Name;

            if (typeof args == "string" || typeof args == "number") {
                return this.Components[args];
            }
            else if (typeof args == "object" && args.length) {
                // 这里 args 是一个数组对象且第一个元素必须为 FrameUI控件对象/jQuery选择器的(ID/TagName)/jQuery Dom对象
                // $(args[0]).attr(id):这里表示获取标签DOM元素的frame属性值，该属性值对应FrameUI控件的ID
                if (!args[0][id] && !$(args[0]).attr(id))
                    return null;
                return this.Components[args[0][id] || $(args[0]).attr(id)];
            }

            return null;
        },
        // 根据类型在组件管理器中查找获取FrameUI控件对象
        // @param [type]：FrameUI控件对象类型,当查找的FrameUI控件对象为不同类型时,使用Array数组指定多个类型参数。
        Find: function (type) {
            var arr = [];
            for (var id in this.Components) {
                var component = this.Components[id];
                if (type instanceof Function) {
                    if (component instanceof type) {
                        arr.push(component);
                    }
                }
                else if (type instanceof Array) {
                    // 检测 component._GetType() 在数组 type 中的位置
                    // 查找不到指定值时返回-1
                    if ($.inArray(component._GetType(), type) != -1) {
                        // 进入这里表示查找到指定值
                        arr.push(component);
                    }
                }
                else if (component._GetType() == type) {
                    arr.push(component);
                }

            }
            return arr;
        },
        Do: function (plugin, args, extension) {
            /// <summary>
            ///  $.fn.Frame{Plugin} 和 $.fn.FrameGet{Plugin}Component
            ///  会调用这个方法,并传入作用域(this)
            ///</summary>
            ///<param name="plugin" type="string">插件名</param>
            ///<param name="args" type="array">参数</param>
            ///<param name="extension" type="object">扩展参数,定义命名空间或者id属性名</param>
            if (!plugin)
                return;
            extension = $.extend({
                DefaultNamespace: "Defaults",
                MethodNamespace: "Methods",
                ControlNamespace: "Controls",
                IdAttr: null,
                IsStatic: false,
                HasElement: true,           //是否拥有element主体(比如Drag、Resizable等辅助性插件就不拥有)
                PropertyToElement: null      //链接到element的属性名
            }, extension || {});

            // 当使用该形式$.{Class}声明定义对象时执行以下代码段
            if (null == this || window == this || extension.IsStatic) {
                if (!$.Frame.PlugIns[plugin]) {
                    // 这里定义声明了jQuery对象的插件对象，例如 $.Window
                    $.Frame.PlugIns[plugin] = {
                        fn: $[plugin],
                        IsStatic: true
                    };
                }

                var options = $.extend({}, $.Frame[extension.DefaultNamespace][plugin] || {}, args.length > 0 ? args[0] : {});
                //$.Frame.Controls.{plugin}(options)
                return new $.Frame[extension.ControlNamespace][plugin](options);
            }

            // 当使用该形式$.fn.{Class}声明定义对象时执行以下代码段
            if (!$.Frame.PlugIns[plugin]) {
                // 这里定义声明了jQuery封装COM对象的插件对象，例如 $.fn.Draggable
                $.Frame.PlugIns[plugin] = {
                    fn: $.fn[plugin],
                    IsStatic: false
                };
            }

            this.each(function () {
                // 已经执行过,即已存在在$.Frame.Components组件管理器中 
                if (null != extension.IdAttr && (this[extension.IdAttr] || $(this).attr(extension.IdAttr))) {
                    var component = $.Frame.Get(this[extension.IdAttr] || $(this).attr(extension.IdAttr));
                    if (component && args.length > 0)
                        component.Set(args[0]);
                    return;
                }
                if (null != args && args.length >= 1 && typeof args[0] == "string")
                    return;

                //只要第一个参数不是string类型,都执行组件的实例化工作
                var options = (null != args && args.length > 0) ? args[0] : null;
                var p = $.extend({}, $[extension.DefaultNamespace][plugin] || {}, options || {},
                    { PropertyToElement: extension.PropertyToElement });

                if (extension.PropertyToElement)
                    p[extension.PropertyToElement] = this; // 这里将标签的jQuery对象存在指定名称的对象中
                var o = {};
                if (extension.HasElement) {
                    // new $.Frame.Controls.{PlugIn}(this,p);
                    new $.Frame[extension.ControlNamespace][plugin](this, p);
                }
                else {
                    // new $.Frame.Controls.{PlugIn}(p);
                    new $.Frame[extension.ControlNamespace][plugin](p);
                }
            });


            if (this.length == 0)
                return null;
            if (null == args || args.length == 0)
                return $.Frame.Get(this, extension.IdAttr);
            if (typeof args[0] == "object")
                return $.Frame.Get(this, extension.IdAttr);
            if (typeof args[0] == "string") {
                var component = $.Frame.Get(this, extension.IdAttr);
                if (component == null)
                    return;
                if (args[0] == "Options") {
                    if (args.length == 2)
                        return component.Get(args[1]);
                    else if (args.length >= 3)
                        return component.Set(args[1], args[2]);
                }
                else {
                    var method = args[0];
                    if (!component[method])
                        return; //不存在这个方法
                    var parms = Array.apply(null, args);
                    parms.shift();
                    return component[method].apply(component, parms);
                }
            }

            return null;
        }
    };

    //扩展对象
    $.Defaults = {};
    $.Methods = {};
    //关联起来
    $.Frame.Defaults = $.Defaults;
    $.Frame.Methods = $.Methods;
    $.Frame.Draggable = { Dragging: false };
    $.Frame.Resizable = { Reszing: false };

    // 获取FrameUI对象
    // @param [plugin]  插件名,可为空
    $.fn.Frame = function (plugin) {
        if (plugin)
            return $.Frame.Do.call(this, plugin, arguments);
        else
            return $.Frame.Get(this);
    };

    // 组件基类,声明定义所有FrameUI控件的基本属性和方法
    // @param [options]：初始化配置参数对象。
    $.Frame.Component = function (options) {
        //事件容器
        this.Events = this.Events || {};
        //配置参数
        this.Options = options || {};
        //子组件集合索引
        this.Children = {};
        //组件依赖的上下文环境
        this.Context = null;
    };
    $.extend($.Frame.Component.prototype, {
        _GetType: function () {
            return "Component";
        },
        _IdPrev: function () {
            return "Component";
        },
        // 设置属性或事件
        // @param :
        // 1.arg 属性名    value 属性值 
        // 2.arg {属性名:值}   value(可选) 是否只设置事件
        Set: function (arg, value) {
            if (!arg) return;

            // 第2种参数情况
            if (typeof arg == "object") {
                var tmp;
                if (this.Options != arg) {
                    $.extend(this.Options, arg);
                    tmp = arg;
                }
                else {
                    tmp = $.extend({}, arg);
                }

                // 为arg对象设置事件
                if (value == undefined || value == true) {
                    for (var o in tmp) {
                        if (o.indexOf("On") == 0)
                            this.Set(o, tmp[o]);
                    }
                }
                // 为arg对象设置属性
                if (value == undefined || value == false) {
                    for (var p in tmp) {
                        if (p.indexOf("On") != 0)
                            this.Set(p, tmp[p]);
                    }
                }
                return;
            }

            // 第一种参数情况
            var name = arg;
            // 事件参数
            // 设置事件
            if (name.indexOf("On") == 0) {
                if (typeof value == "function")
                    this.Bind(name.substr(2), value);
                return;
            }

            // 属性设置有事件OnPropertyChange时生效
            if (this.Trigger("PropertyChange", [arg, value]) == false) return;

            // 设置属性,并调用对应FrameUI控件对象的设置属性函数
            this.Options[name] = value;
            var pn = "_Set" + name.substr(0, 1).toUpperCase() + name.substr(1);
            if (this[pn]) {
                this[pn].call(this, value);
            }

            this.Trigger("PropertyChange", [arg, value]);
        },
        // 获取属性值
        // @param [name]：要获取值的属性名称或获取属性的函数名称。
        // @param [args](可选)：当name为获取属性的函数名称，这里则为传入函数的参数对象。
        Get: function (name, args) {
            var pn = "_Get" + name.substr(0, 1).toUpperCase() + name.substr(1);
            if (this[pn]) {
                return this[pn].call(this, args);
            }
            return this.Options[name];
        },
        // 触发事件
        // @param [arg]：触发的事件名称。
        // @param [data](可选)：传递给事件处理函数的附加参数,一般为数组类型。
        Trigger: function (arg, data) {
            var name = arg.toString();
            var event = this.Events[name];
            if (!event) return;
            data = data || [];
            if ((data instanceof Array) == false) {
                data = [data];
            }
            for (var i = 0; i < event.length; i++) {
                var ev = event[i];
                var result = ev.Handler.apply(ev.Context, data);
                if (result == false)
                    return false;
                else
                    return result;
            }
        },
        // 检测是否有绑定指定名称的事件函数
        // @param [arg]：事件函数名称。
        HasBind: function (arg) {
            var name = arg.toString();
            var event = this.Events[name];
            if (event && event.length)
                return true;
            else
                return false;
        },
        // 为当前FrameUI控件对象绑定相应事件，将调用的事件函数加入事件容器 Events 中。
        // @param [arg]：绑定事件主要参数。
        //               若为对象类型,结构为{事件名(不带On):function()};
        //               若为字符串类型,即直接指定事件名，则必须传入指定第二个参数 handler。 
        // @param [handler](可选)：触发事件时调用的函数方法。
        // @param [context](可选)：事件执行时的上下文对象。若无直接指定，则以当前对象作为其上下文对象。
        Bind: function (arg, handler, context) {
            if (typeof arg == "object") {
                for (var p in arg) {
                    this.Bind(p, arg[p]);
                }
                return;
            }
            if (typeof handler != "function")
                return false;
            var name = arg.toString();
            var event = this.Events[name] || [];
            context = context || this;
            event.push({ Handler: handler, Context: context });
            this.Events[name] = event;
        },
        // 从事件队列中移除指定名称事件
        // @param [arg]：要移除的事件名称。若该参数未指定，则将事件全部清除。
        // @param [handler](可选)：要移除的事件调用函数。
        Unbind: function (arg, handler) {
            if (!arg) {
                this.Events = {};
                return;
            }
            var name = arg.toString();
            var event = this.Events[name];
            if (!event || !event.length) return;
            if (!handler) {
                delete this.Events[name];
            }
            else {
                for (var i = 0, l = event.length; i < l; i++) {
                    if (event[i].Handler == handler) {
                        event.splice(i, 1);
                        break;
                    }
                }
            }
        },
        // 销毁函数
        Destroy: function () { }
    });

    //界面组件基类, 
    //1,完成界面初始化:设置组件id并存入组件管理器池,初始化参数
    //2,渲染的工作,细节交给子类实现
    //@param [element] 组件对应的dom element对象
    //@param [options] 组件的参数
    $.Frame.UIComponent = function (element, options) {
        $.Frame.UIComponent.Parent.constructor.call(this, options);
        var extendMethods = this._ExtendMethods();
        if (extendMethods) $.extend(this, extendMethods);
        this.Element = element;
        this._Initializa();
        this._PreRender();
        this.Trigger("Render");
        this._Render();
        this.Trigger("Rendered");
        this._Rendered();
    };
    $.Frame.UIComponent.Extension($.Frame.Component, {
        _GetType: function () {
            return "UIComponent";
        },
        //扩展方法
        _ExtendMethods: function () {

        },
        _Initializa: function () {
            this.Type = this._GetType();
            if (!this.Element) {
                this.Id = this.Options.Id || $.Frame.CreateId(this._IdPrev());
            }
            else {
                this.Id = this.Options.Id || this.Element.id || $.Frame.CreateId(this._IdPrev());
            }
            //存入管理器池
            $.Frame.Add(this);

            if (!this.Element)
                return;

            //读取Attr方法,并加载到参数,比如["url"]
            var attributes = this.Attr();
            if (attributes && attributes instanceof Array) {
                for (var i = 0; i < attributes.length; i++) {
                    var name = attributes[i];
                    this.Options[name] = $(this.Element).attr(name);
                }
            }
            // 读取FrameUI控件链接的Dom标签元素这个属性，并加载到Options参数中，
            // 比如 frame-options = "width:120,heigth:100"
            var p = this.Options;
            if ($(this.Element).attr("frame-options")) {
                try {
                    var attroptions = $(this.Element).attr("frame-options");
                    if (attroptions.indexOf("{") != 0)
                        attroptions = "{" + attroptions + "}";

                    if (attroptions)
                        $.extend(p, (new Function("return " + attroptions))());
                }
                catch (e) { }
            }
        },
        //预渲染,可以用于继承扩展
        _PreRender: function () {

        },
        _Render: function () {

        },
        _Rendered: function () {
            if (this.Element) {
                $(this.Element).attr("frame", this.Id);
                //                if (!$(this.Element).attr("id")) {
                //                    $(this.Element).attr("id", this.Id);
                //                }
            }
            if (this[this.Options.PropertyToElement]) {
                $(this[this.Options.PropertyToElement]).attr("frame", this.Id);
            }
        },
        //返回要转换成Frame参数的属性,比如["url"]
        Attr: function () {
            return [];
        },
        Destroy: function () {
            if (this.Element)
                $(this.Element).remove();
            this.Options = null;
            $.Frame.Remove(this);
        }
    });

    $.Frame.Controls.Base = function (element, options) {
        $.Frame.Controls.Base.Parent.constructor.call(this, element, options);
    };
    $.Frame.Controls.Base.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Base";
        },
        Attr: function () {
            return ["NullText"];
        },
        _IdPrev: function () {
            return "BaseFor";
        },
        SetValue: function (value) {
            return this.Set("Value", value);
        },
        GetValue: function () {
            return this.Get("Value");
        },
        Enable: function () {
            return this.Set("Disabled", false);
        },
        Disable: function () {
            return this.Set("Disabled", true);
        }
    });

    // 提供窗体界面的基础拓展
    $.Frame.WindowBase = {
        Top: false,
        Masking: false,
        WindowMask: null, // 遮罩层
        TaskBar: null,
        Tasks: null,
        AutoHide: true,
        Mask: function (parentNode) {
            if (!this.WindowMask) {
                //                this.WindowMask = $("<div></div>").css("position", "absolute").css("left", "0").css("top", "0").css("width", "100%")
                //                    .css("height", "100%").css("filter", "alpha(opacity=25)").css("opacity", "0.25").css("background", "#ADADAD")
                //                    .css("font-size", "1px").css("zoom", "1").css("overflow", "hidden").css("display", "none").css("z-index", "9000")
                //                    .appendTo((parent ? $(parent)[0] : "body"));
                //                $(window).bind("resize.Window", this._SetHeight);
                //                $(window).bind("scroll", this._SetHeight);

                this.WindowMask = $("<div></div>").addClass("frame-modal").addClass("in");
                this.WindowMask.appendTo((parentNode ? parentNode : "body"));
//                if (self.frameElement && self.frameElement.tagName == "IFRAME") {
//                    if ($(".frame-modal", (parentNode ? parentNode : parent.window.document.body)).length > 0)
//                        this.WindowMask = $(".frame-modal", (parentNode ? parentNode : parent.window.document.body));
//                    else {
//                        this.WindowMask = $("<div></div>").addClass("frame-modal").addClass("in");
//                        this.WindowMask.appendTo((parentNode ? parentNode : parent.window.document.body));
//                    }
//                } else {
//                    this.WindowMask = $("<div></div>").addClass("frame-modal").addClass("in");
//                    this.WindowMask.appendTo((parentNode ? parentNode : "body"));
//                }
            }
            this.WindowMask.show();
            //this._SetHeight();
            this.Masking = true;
        },
        UnMask: function (win) {
//            var jwins = self.frameElement ? $(".frame-formbox:visible", parent.window.document.body) : $("body > .frame-formbox:visible");
            var jwins = $("body > .frame-formbox:visible");
            for (var index = 0, count = jwins.length; index < count; index++) {
                var id = jwins.eq(index).attr("frame");
                if (win && win.Id == id) continue;

                // 获取Frame的Components组件管理对象池中的FormBox窗口对象$.FormBox
                var wincomponent = $.Frame.Get(id);
                if (!wincomponent) continue;

                // 是否为模式窗口 
                // 获取$.FormBox中的IsModal属性
                var isModal = wincomponent.Get("IsModal");
                //如果存在其他模态窗口，那么不会取消遮罩
                if (isModal) return;
            }

            if (this.WindowMask)
                this.WindowMask.hide();
            this.Masking = false;
        },
        SetFront: function (win) {
            var windows = $.Frame.Find($.Frame.Controls.WindowBase);
            for (var index in windows) {
                var w = windows[index];
                if (w == win)
                    $(w.Element).css("z-index", "9200");
                else
                    $(w.Element).css("z-index", "9100");
            }
        },
        CreateTaskBar: function (parent) {
            if (!this.TaskBar) {
                this.TaskBar = $("<div></div>").addClass("frame-taskbar")
                    .append($("<div></div>").addClass("frame-taskbar-tasks"))
                    .append($("<div></div>").addClass("frame-clear"))
                    .appendTo((parent ? $(parent)[0] : "body"));
                if (this.Top)
                    this.TaskBar.addClass("frame-taskbar-top");
                this.TaskBar.Tasks = $(".frame-taskbar-tasks:first", this.TaskBar);
                this.Tasks = {};
            }

            this.TaskBar.show();
            this.TaskBar.animate({ bottom: 0 });
            return this.TaskBar;
        },
        HideTaskBar: function () {
            var self = this;
            self.TaskBar.animate({ bottom: -38 }, function () {
                self.TaskBar.remove();
                self.TaskBar = null;
            });
        },
        ActiveTask: function (win) {
            for (var id in this.Tasks) {
                var task = this.Tasks[id];
                if (id == win.Id)
                    task.addClass("frame-taskbar-task-active");
                else
                    task.removeClass("frame-taskbar-task-active");
            }
        },
        GetTask: function (win) {
            var self = this;
            if (!self.TaskBar)
                return null;
            if (self.Tasks[win.Id])
                return self.Tasks[win.Id];
            return null;
        },
        AddTask: function (win) {
            var self = this;
            if (!self.TaskBar)
                self.CreateTaskBar();
            if (self.Tasks[win.Id])
                return self.Tasks[win.Id];

            var title = win.Get("Title");
            var task = self.Tasks[win.Id] = $("<div></div>").addClass("frame-taskbar-task")
                .append($("<div></div>").addClass("frame-taskbar-task-icon"))
                .append($("<div></div>").addClass("frame-taskbar-task-content").text(title));
            self.TaskBar.Tasks.append(task);
            self.ActiveTask(win);

            task.bind("click", function () {
                self.ActiveTask(win);
                if (win._Actived)
                    win.Minimize();
                else
                    win.Active();
            }).hover(function () {
                $(this).addClass("frame-taskbar-task-over");
            }, function () {
                $(this).removeClass("frame-taskbar-task-over");
            });
            return task;
        },
        RemoveTask: function (win) {
            var self = this;
            if (!self.TaskBar)
                return;
            if (self.Tasks[win.Id]) {
                self.Tasks[win.Id].unbind();
                self.Tasks[win.Id].remove();
                delete self.Tasks[win.Id];
            }
            if (!self.HasTask() && self.AutoHide) {
                self.HideTaskBar();
            }
        },
        HasTask: function () {
            for (var p in this.Tasks) {
                if (this.Tasks[p])
                    return true;
            }
            return false;
        },
        _SetHeight: function () {
            if (!$.Frame.WindowBase.WindowMask)
                return;
            var h = $(window).height() + $(window).scrollTop();
            $.Frame.WindowBase.WindowMask.height(h);
        }
    };

    // 窗体控件的基类
    $.Frame.Controls.WindowBase = function (element, options) {
        $.Frame.Controls.WindowBase.Parent.constructor.call(this, element, options);
    };
    $.Frame.Controls.WindowBase.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "WindowBase";
        },
        Mask: function () {
            if (this.Options.IsModal)
                $.Frame.WindowBase.Mask();
        },
        UnMask: function () {
            if (this.Options.IsModal)
                $.Frame.WindowBase.UnMask(this);
        },
        Minimize: function () {
        },
        Maximize: function () {
        },
        Active: function () {
        }
    });

    $.Frame.Config = {
        IsAutoParser: false,
        UseTaskBar: false,
        Do: function () {
            if (this.IsAutoParser) {
                $.Parser.Auto = this.IsAutoParser;
                $.Parser.Do();
            }
            if (this.UseTaskBar) {
                $.Frame.WindowBase.AutoHide = (!this.UseTaskBar);
                $.Frame.WindowBase.CreateTaskBar();
            }
        }
    };

    $.extend(Date.prototype, {
        Format: function (format) {
            var o = {
                "M+": this.getMonth() + 1, //月份 
                "d+": this.getDate(), //日 
                "h+": this.getHours(), //小时 
                "m+": this.getMinutes(), //分 
                "s+": this.getSeconds(), //秒 
                "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
                "S": this.getMilliseconds() //毫秒 
            };
            if (/(y+)/.test(format))
                format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
            for (var k in o) {
                if (new RegExp("(" + k + ")").test(format))
                    format = format.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
            }
            return format;
        },
        AddYear: function (value, date) {
            if (date == null)
                date = this;
            date.setMonth(date.getMonth() + (value * 12));

            return date;
        },
        AddMonth: function (value, date) {
            if (date == null)
                date = this;
            date.setMonth(date.getMonth() + value);

            return date;
        },
        AddDay: function (value, date) {
            if (date == null)
                date = this;
            date.setDate(date.getDate() + value);

            return date;
        }
    });

    $.iFrameResize = function (iframe) {
        if (iframe) {
            var doc = iframe.contentDocument;
            if (doc != null && doc.body != null) {
                $(iframe).height(doc.body.scrollHeight);
            }
        }
    };
})(jQuery);

//jQuery自定义resize函数,解决普通元素的jquery resize事件
(function ($, window, undefined) {
    var elems = $([]),
    jq_resize = $.resize = $.extend($.resize, {}),
    timeout_id,
    str_setTimeout = 'setTimeout',
    str_resize = 'resize',
    str_data = str_resize + '-special-event',
    str_delay = 'delay',
    str_throttle = 'throttleWindow';
    jq_resize[str_delay] = 250;
    jq_resize[str_throttle] = true;
    $.event.special[str_resize] = {
        setup: function () {
            if (!jq_resize[str_throttle] && this[str_setTimeout]) {
                return false;
            }
            var elem = $(this);
            elems = elems.add(elem);
            $.data(this, str_data, {
                w: elem.width(),
                h: elem.height()
            });
            if (elems.length === 1) {
                loopy();
            }
        },
        teardown: function () {
            if (!jq_resize[str_throttle] && this[str_setTimeout]) {
                return false;
            }
            var elem = $(this);
            elems = elems.not(elem);
            elem.removeData(str_data);
            if (!elems.length) {
                clearTimeout(timeout_id);
            }
        },
        add: function (handleObj) {
            if (!jq_resize[str_throttle] && this[str_setTimeout]) {
                return false;
            }
            var old_handler;
            function new_handler(e, w, h) {
                var elem = $(this),
          data = $.data(this, str_data);
                data.w = w !== undefined ? w : elem.width();
                data.h = h !== undefined ? h : elem.height();
                old_handler.apply(this, arguments);
            }
            if ($.isFunction(handleObj)) {
                old_handler = handleObj;
                return new_handler;
            } else {
                old_handler = handleObj.handler;
                handleObj.handler = new_handler;
            }
        }
    };

    function loopy() {
        timeout_id = window[str_setTimeout](function () {
            elems.each(function () {
                var elem = $(this),
          width = elem.width(),
          height = elem.height(),
          data = $.data(this, str_data);
                if (width !== data.w || height !== data.h) {
                    elem.trigger(str_resize, [data.w = width, data.h = height]);
                }
            });
            loopy();
        }, jq_resize[str_delay]);
    }
})(jQuery, this);