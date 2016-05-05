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
                this.WindowMask = $("<div></div>").addClass("frame-modal").addClass("in");
                this.WindowMask.appendTo((parentNode ? parentNode : "body"));
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

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.01
* 说明：Alert 提示对话框控件。
*/
(function ($) {
    $.fn.Alert = function (options) {
        return $.Frame.Do.call(this, "Alert", arguments);
    };

    $.Methods.Alert = $.Methods.Alert || {};

    $.Defaults.Alert = {
        Width: null,
        Mode: "success",
        Icon: null,
        Size: null,     // small,large
        Content: "",
        Closable: false
    };

    $.Frame.Controls.Alert = function (element, options) {
        $.Frame.Controls.Alert.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Alert.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Alert";
        },
        _IdPrev: function () {
            return "AlertFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Alert;
        },
        _PreRender: function () {
            var alerter = this;

            if (!$(alerter.Element).hasClass("frame-alert")) {
                $(alerter.Element).addClass("frame-alert");
            }

            if ($(alerter.Element).text().length > 0) {
                this.Options.Content = $(alerter.Element).html();
                $(alerter.Element).empty();
            }
        },
        _Render: function () {
            var alerter = this, options = this.Options;
            //glyphter-close
            $("<button></button>").addClass("close").append('<i class="icon-remove"></i>')
                .appendTo(alerter.Element).on("click", function () { alerter.Close(); return false; });
            $("<p></p>").appendTo(alerter.Element);

            alerter.Set(options);

            if (options.Closable)
                $(alerter.Element).hide();
        },
        _SetWidth: function (width) {
            var alerter = this, options = this.Options;
            if (width) {
                $(alerter.Element).width(width);
            }
        },
        _SetMode: function (mode) {
            var alerter = this, options = this.Options;
            if (mode) {
                $(alerter.Element).addClass("frame-alert-" + mode);
            }
        },
        _SetSize: function (size) {
            var alerter = this, options = this.Options;
            if (size) {
                $(alerter.Element).addClass("frame-alert-" + size);
            }
        },
        _SetIcon: function (icon) {
            var alerter = this, options = this.Options;
            if (icon) {
                $("p", alerter.Element).append($("<i></i>").addClass(icon));
            }
        },
        _SetContent: function (content) {
            var alerter = this, options = this.Options;

            var detail = ("function" == typeof content ? content.call(alerter.Element) : content);
            $("p", alerter.Element).append(detail);
        },
        IsVisible: function () {
            var alerter = this;
            return $(alerter.Element).is(":visible");
        },
        Show: function () {
            var alerter = this;
            $(alerter.Element).show();
        },
        Close: function () {
            var alerter = this;
            $(alerter.Element).hide();
        },
        SetContent: function (content) {
            var alerter = this, options = this.Options;
            $("p", alerter.Element).empty();
            alerter.Set({ Icon: options.Icon, Content: content });
        },
        Toggle: function () {
            var alerter = this;
            alerter.IsVisible() == false ? alerter.Show() : alerter.Close();
        }
    });

})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.06.14
* 说明：AppButton 按钮控件。
*/
(function ($) {
    $.fn.AppButton = function (options) {
        return $.Frame.Do.call(this, "AppButton", arguments);
    };

    $.Methods.AppButton = $.Methods.AppButton || {};

    $.Defaults.AppButton = {
        Text: "",
        Size: null, // normal，small
        Mode: "primary",
        Icon: null,
        BookMark: null,
        BookMarkSite: null,
        Disabled: false,
        OnClick: null
    };

    $.Frame.Controls.AppButton = function (element, options) {
        $.Frame.Controls.AppButton.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.AppButton.Extension($.Frame.Controls.Base, {
        _GetType: function () {
            return "AppButton";
        },
        _IdPrev: function () {
            return "AppButtonFor";
        },
        _ExtendMethods: function () {
            return $.Methods.AppButton;
        },
        _PreRender: function () {
            var app = this;

            if (!$(app.Element).hasClass("frame-button-app")) {
                $(app.Element).addClass("frame-button-app");
            }
            $(app.Element).addClass("frame-button");
        },
        _Render: function () {
            var app = this, options = this.Options;

            // 将标签的文本值迁移到构造的文本标签中，这里一般发生在按钮对象使用javascript创建的情况
            options.Text = options.Text || $(app.Element).text();
            if ($(app.Element).attr("disabled") && !options.Disabled)
                options.Disabled = true;
            $(app.Element).empty();

            $(app.Element).on("click", function () {
                if (app.Disabled)
                    return;
                if (app.HasBind("Click")) {
                    app.Trigger("Click", [this]);
                }

                return false;
            });

            app.Set(options);
        },
        _SetText: function (text) {
            var app = this, options = this.Options;
            app._SetValue(text);
        },
        _SetValue: function (value) {
            var app = this, options = this.Options;
            if (value) {
                var span = $("span", app.Element);
                if (span.length > 0) {
                    span.text(value);
                }
                else
                    $(app.Element).append($("<span></span>").text(value));
            }
        },
        _GetValue: function () {
            var app = this, options = this.Options;
            var textElement = $("span", app.Element);
            if (textElement.length > 0)
                return textElement.text();
            else
                return "";
        },
        _SetSize: function (size) {
            ///<summary>
            /// 设置按钮的尺寸样式
            ///</summary>
            ///<param name="size" type="string">尺寸类型.[normal,samll]</param>
            var app = this, options = this.Options;

            if (size) {
                $(app.Element).addClass("frame-button-" + size);
            }
        },
        _SetMode: function (mode) {
            ///<summary>
            /// 设置按钮的类型样式
            ///</summary>
            ///<param name="mode" type="string">按钮类型.[original,default,primary,info,success,error,inverse,warning]</param>
            var app = this, options = this.Options;
            if (mode != "original") {
                $(app.Element).addClass("frame-button-" + mode);
            }
        },
        _SetDisabled: function (disabled) {
            var app = this, options = this.Options;
            app.Disabled = disabled;
            if (disabled) {
                $(app.Element).attr("disabled", "disabled");
                $(app.Element).addClass("disabled");
            } else {
                $(app.Element).removeAttr("disabled");
                $(app.Element).removeClass("disabled");
            }
        },
        _GetDisabled: function () {
            var app = this, options = this.Options;
            return app.Disabled;
        },
        _SetIcon: function (icon) {
            var app = this, options = this.Options;
            if (icon) {
                var i = $("<i></i>").addClass(icon);
                $(app.Element).prepend(i);
            }
        },
        _SetBookMark: function (bookmark) {
            var app = this, options = this.Options;
            if (bookmark) {
                if (options.BookMarkSite && options.BookMarkSite == "left") {
                    if (bookmark._GetType() == "Label")
                        $(bookmark.Element).addClass("label-left");
                    if (bookmark.GetType() == "Badge")
                        $(bookmark.Element).addClass("badge-left");
                }
                app.BookMark = bookmark;
                $(app.Element).append(bookmark.Element);
            }
        },
        SetBookMarkText: function (text) {
            var app = this, options = this.Options;
            if (app.BookMark) {
                app.BookMark.Set("Text", text);
            }
        }
    });

})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.06.14
* 说明：Badge 标记控件。
*/
(function ($) {
    $.fn.Badge = function (options) {
        return $.Frame.Do.call(this, "Badge", arguments);
    };

    $.Methods.Badge = $.Methods.Badge || {};

    $.Defaults.Badge = {
        Text: "",
        Mode: "primary"
    };

    $.Frame.Controls.Badge = function (element, options) {
        $.Frame.Controls.Badge.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Badge.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Badge";
        },
        _IdPrev: function () {
            return "BadgeFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Badge;
        },
        _PreRender: function () {
            var badge = this;

            if (!$(badge.Element).hasClass("frame-badge")) {
                $(badge.Element).addClass("frame-badge");
            }
        },
        _Render: function () {
            var badge = this, options = this.Options;

            options.Text = options.Text || $(badge.Element).text();
            $(badge.Element).empty();

            badge.Set(options);
        },
        _SetText: function (text) {
            var badge = this, options = this.Options;
            $(badge.Element).text(text);
        },
        _GetText: function () {
            var badge = this, options = this.Options;
            return $(badge.Element).text();
        },
        _SetMode: function (mode) {
            ///<summary>
            /// 设置标记的类型样式
            ///</summary>
            ///<param name="mode" type="string">标记类型.[primary,info,success,error,inverse,warning,important]</param>
            var badge = this, options = this.Options;
            if(mode)
                $(badge.Element).addClass("frame-badge-" + mode);
        }
    });

})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.20
* 说明：BreadCrumbs 面包屑控件。
*/
(function ($) {
    $.fn.BreadCrumbs = function (options) {
        return $.Frame.Do.call(this, "BreadCrumbs", arguments);
    };

    $.Methods.BreadCrumbs = $.Methods.BreadCrumbs || {};

    $.Defaults.BreadCrumbs = {
        Items: [],         //{Url,Params,Text}
        ToolBar: null,
        OnItemClick: null
    };

    $.Frame.Controls.BreadCrumbs = function (element, options) {
        $.Frame.Controls.BreadCrumbs.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.BreadCrumbs.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "BreadCrumbs";
        },
        _IdPrev: function () {
            return "BreadCrumbsFor";
        },
        _ExtendMethods: function () {
            return $.Methods.BreadCrumbs;
        },
        _PreRender: function () {
            var breadcrumbs = this;

            if (!$(breadcrumbs.Element).hasClass("frame-breadcrumbs")) {
                $(breadcrumbs.Element).addClass("frame-breadcrumbs");
            }
        },
        _Render: function () {
            var breadcrumbs = this, options = this.Options;
            breadcrumbs.Container = $("<ul></ul>").addClass("frame-breadcrumb").appendTo(breadcrumbs.Element);

            breadcrumbs.Set(options);
        },
        _SetItems: function (items) {
            var breadcrumbs = this, options = this.Options;

            if (items && items.length > 0) {
                $(items).each(function (i) {
                    breadcrumbs._Append(i, this);
                });
                $(breadcrumbs.Container).children().last().find("a").trigger("click");
                $(breadcrumbs.Container).children().last().addClass("active");
            }
        },
        _SetToolBar: function (tools) {
            var breadcrumbs = this, options = this.Options;
            if (tools && tools.length > 0) {
                var toolbar = $("<div></div>").addClass("frame-breadcrumb-toolbar").appendTo(breadcrumbs.Element);
                //                $(tools).each(function () {
                //                    toolbar.append($(this.Element));
                //                });
                $(tools).each(function () {
                    switch (this.Type) {
                        case "Button":
                            var button = $("<button></button>").Button({
                                Mode: "transparent",
                                Size: "large",
                                Icon: this.Icon,
                                IconSite: "only",
                                OnClick: this.OnClick
                            });
                            toolbar.append(button.Element);
                            this.Desc && $(button.Element).Tip({ Placement: "auto", Mode: "inverse", Title: this.Desc });
                            break;
                        case "TextBox":
                            var textbox = $('<input type="text" />').TextBox({
                                Icon: this.Icon,
                                Size: "small",
                                Width: this.Width,
                                OnBeforeFocus: this.OnBeforeFocus,
                                OnFocus: this.OnFocus,
                                OnBlur: this.OnBlur,
                                OnChange: this.OnChange
                            });
                            toolbar.append($(textbox.Element).parent());
                            this.Desc && $(textbox.Element).parent().Tip({ Placement: "auto", Mode: "inverse", Title: this.Desc });
                            break;
                    }
                });
            }
        },
        _Join: function (url, params) {
            if (url) {
                url += url.indexOf('?') == -1 ? "?" : "&";
                for (var name in params) {
                    url += (name + "=" + params[name] + "&");
                }
                url += ("random=" + new Date().getTime());
            } else
                url = "#";
            return url;
        },
        Append: function (item) {
            var breadcrumbs = this, options = this.Options;
            var index = options.Items.length;
            options.Items.push(item);
            $("li.active", breadcrumbs.Container).removeClass("active");
            breadcrumbs._Append(index, item);
            $(breadcrumbs.Container).children().last().find("a").trigger("click");
            $(breadcrumbs.Container).children().last().addClass("active");
        },
        AppendRange: function (items) {
            var breadcrumbs = this, options = this.Options;
            if (items && items.length > 0) {
                $("li.active", breadcrumbs.Container).removeClass("active");
                $(items).each(function () {
                    var index = options.Items.length;
                    options.Items.push(this);
                    breadcrumbs._Append(index, this);
                });
                $(breadcrumbs.Container).children().last().find("a").trigger("click");
                $(breadcrumbs.Container).children().last().addClass("active");
            }
        },
        _Append: function (index, item) {
            var breadcrumbs = this, options = this.Options;

            var itemElement = $("<li></li>").appendTo(breadcrumbs.Container);
            var url = breadcrumbs._Join(item.Url, item.Params);
            if (index == 0) {
                $("<i></i>").addClass("icon-home").addClass("home-icon").appendTo(itemElement);
            }
            $("<a></a>").attr("idx", index).text(item.Text).attr("href", url).appendTo(itemElement)
                            .on("click", function (e) {
                                if ($(this).parent().hasClass("active"))
                                    return false;
                                var r = true;
                                if (breadcrumbs.HasBind("ItemClick")) {
                                    var index = parseInt($(this).attr("idx"));
                                    var d = options.Items[index];
                                    r = breadcrumbs.Trigger("ItemClick", [this, d]);
                                }
                                if (r)
                                    breadcrumbs._Remove(this, index);
                                return false;
                            });
        },
        _Remove: function (sender, index) {
            var breadcrumbs = this, options = this.Options;
            options.Items.splice(index + 1, options.Items.length - 1);

            $(breadcrumbs.Container).children().each(function (i) {
                if (i > index)
                    $(this).remove();
            });
            $(sender).parent().addClass("active");
        },
        Clear: function () {
            var breadcrumbs = this, options = this.Options;
            options.Items.length = 0;
            $(breadcrumbs.Container).empty();
        }
    });

})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.06.13
* 说明：Button 按钮控件。
*/
(function ($) {
    $.fn.Button = function (options) {
        return $.Frame.Do.call(this, "Button", arguments);
    };

    $.Methods.Button = $.Methods.Button || {};

    $.Defaults.Button = {
        Text: "",
        Size: null,
        Mode: "primary",
        CanBlock: false,
        Actived: false,
        Icon: null,
        IconSite: "left", // left,right,side,only
        Width: null,
        Height: null,
        Disabled: false,
        OnClick: null
    };

    $.Frame.Controls.Button = function (element, options) {
        $.Frame.Controls.Button.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Button.Extension($.Frame.Controls.Base, {
        _GetType: function () {
            return "Button";
        },
        _IdPrev: function () {
            return "ButtonFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Button;
        },
        _PreRender: function () {
            var button = this;

            // 当不是通过标签创建FrameUI的Button对象时，即当 $.Parser.Auto=false 时,标签不会自动添加frame-button样式
            if (!$(button.Element).hasClass("frame-button")) {
                $(button.Element).addClass("frame-button");
            }
        },
        _Render: function () {
            var button = this, options = this.Options;

            // 将标签的文本值迁移到构造的文本标签中，这里一般发生在按钮对象使用javascript创建的情况
            options.Text = options.Text || $(button.Element).text();
            if ($(button.Element).attr("disabled") && !options.Disabled)
                options.Disabled = true;
            $(button.Element).empty();

            $(button.Element).on("click", function () {
                if (button.Disabled)
                    return;
                if (button.HasBind("Click")) {
                    button.Trigger("Click", [this]);
                }

                return false;
            });

            button.Set(options);
        },
        _SetText: function (text) {
            var button = this, options = this.Options;
            button._SetValue(text);
        },
        _SetValue: function (value) {
            var button = this, options = this.Options;
            if (value) {
                var span = $("span", button.Element);
                if (span.length > 0) {
                    span.text(value);
                }
                else
                    $(button.Element).append($("<span></span>").text(value));
            }
        },
        _GetValue: function () {
            var button = this, options = this.Options;
            var textElement = $("span", button.Element);
            if (textElement.length > 0)
                return textElement.text();
            else
                return "";
        },
        _SetSize: function (size) {
            ///<summary>
            /// 设置按钮的尺寸样式
            ///</summary>
            ///<param name="size" type="string">尺寸类型.[large,normal,samll,smaller]</param>
            var button = this, options = this.Options;

            if (size) {
                $(button.Element).addClass("frame-button-" + size);
            }
        },
        _SetMode: function (mode) {
            ///<summary>
            /// 设置按钮的类型样式
            ///</summary>
            ///<param name="mode" type="string">按钮类型.[original,default,primary,info,success,error,inverse,warning]</param>
            var button = this, options = this.Options;
            if (mode != "original") {
                $(button.Element).addClass("frame-button-" + mode);
            }
        },
        _SetCanBlock: function (block) {
            var button = this, options = this.Options;
            if (block) {
                $(button.Element).addClass("frame-button-block");
            }
        },
        _SetActived: function (actived) {
            var button = this, options = this.Options;
            if (actived)
                $(button.Element).addClass("active");
            else
                $(button.Element).removeClass("active");
        },
        _GetActived: function () {
            var button = this, options = this.Options;
            return $(button.Element).hasClass("active");
        },
        _SetIcon: function (icon) {
            var button = this, options = this.Options;
            if (icon) {
                var i = $("<i></i>").addClass(icon);
                switch (options.IconSite) {
                    case "left": $(button.Element).prepend(i); break;
                    case "right": $(button.Element).append(i.addClass("icon-on-right")); break;
                    case "side":
                        var iLeft = $("<i></i>").addClass(icon[0]);
                        var iRight = $("<i></i>").addClass(icon[1]).addClass("icon-on-right");
                        $(button.Element).prepend(iLeft);
                        $(button.Element).append(iRight);
                        break;
                    case "only": $(button.Element).append(i.addClass("icon-only")); break;
                }
            }
        },
        _SetDisabled: function (disabled) {
            var button = this, options = this.Options;
            button.Disabled = disabled;
            if (disabled) {
                $(button.Element).attr("disabled", "disabled");
                $(button.Element).addClass("disabled");
            } else {
                $(button.Element).removeAttr("disabled");
                $(button.Element).removeClass("disabled");
            }
        },
        _GetDisabled: function () {
            var button = this, options = this.Options;
            return button.Disabled;
        },
        _SetWidth: function (width) {
            var button = this, options = this.Options;
            if (width)
                $(button.Element).width(width);
        },
        _SetHeight: function (height) {
            var button = this, options = this.Options;
            if (height)
                $(button.Element).height(height);

        },
        Actived: function (active) {
            var button = this, options = this.Options;
            button.Set({ Actived: active });
        }
    });


})(jQuery);

/*
* FrameUI v2.1.0
*
* 作者：zlx
* 日期：2015.05.27
* 说明：CheckBox 复选框控件
*
* Copyright 2015 zlx
*
*/
(function ($) {
    $.fn.CheckBox = function (options) {
        return $.Frame.Do.call(this, "CheckBox", arguments);
    };

    $.Methods.CheckBox = $.Methods.CheckBox || {};

    $.Defaults.CheckBox = {
        Value: null,
        Checked: null,
        Disabled: false,
        OnChange: null
    };

    $.Frame.Controls.CheckBox = function (element, options) {
        $.Frame.Controls.CheckBox.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.CheckBox.Extension($.Frame.Controls.Base, {
        _GetType: function () {
            return "CheckBox";
        },
        _IdPrev: function () {
            return "CheckBoxFor";
        },
        _ExtendMethods: function () {
            return $.Methods.CheckBox;
        },
        _PreRender: function () {
            var checkbox = this, options = this.Options;

            // TODO：这里为了防止页面标签元素重复设置样式
            if (!$(checkbox.Element).hasClass("frame-checkbox")) {
                $(checkbox.Element).addClass("frame-checkbox");
            }

            options.Value = options.Value || $(checkbox.Element).val();
            if (options.Value == "on") {
                options.Value = "";
            }
        },
        _Render: function () {
            var checkbox = this, options = this.Options;

            var parent = $(checkbox.Element).parent();
            var wrapper = $("<div></div>").addClass("frame-checkbox-wrapper").appendTo(parent);
            var container = $("<label></label>").appendTo(wrapper);
            $(checkbox.Element).appendTo(container)
            checkbox.Label = $("<span></span>").addClass("lbl").appendTo(container);

            $(checkbox.Element).on("change", function (v) {
                if (checkbox.HasBind("Change"))
                    checkbox.Trigger("Change", [v.target.checked]);
                return false;
            });

            checkbox.Set(options);
        },
        _SetChecked: function (checked) {
            ///<summary>
            /// 设置复选框勾选状态
            ///</summary>
            ///<param name="checked" type="bool">设置一个值,该值标识是否复选框被勾选.</param>
            var checkbox = this, options = this.Options;
            $(checkbox.Element)[0].checked = checked;
        },
        _GetChecked: function () {
            ///<summary>
            /// 获取复选框勾选状态
            ///</summary>
            var checkbox = this, options = this.Options;
            return $(checkbox.Element)[0].checked
        },
        _SetValue: function (value) {
            ///<summary>
            /// 设置复选框显示文本
            ///</summary>
            ///<param name="value" type="string">复选框文本内容.</param>
            var checkbox = this, options = this.Options;
            if (value)
                checkbox.Label.text(value);
        },
        _GetValue: function () {
            ///<summary>
            /// 获取复选框显示文本
            ///</summary>
            var checkbox = this, options = this.Options;
            return checkbox.Label.text();
        },
        _SetDisabled: function (disabled) {
            ///<summary>
            /// 设置复选框禁用状态
            ///</summary>
            ///<param name="disabled" type="bool">设置一个值,该值标识复选框是否被禁用或启用[false:启用,true:禁用].</param>
            var checkbox = this, options = this.Options;
            $(checkbox.Element).attr("disabled", disabled);
        },
        _GetDisabled: function () {
            ///<summary>
            /// 获取复选框状态
            ///</summary>
            var checkbox = this, options = this.Options;
            return $(checkbox.Element).attr("disabled");
        },
        Checked: function (checked) {
            ///<summary>
            /// 设置复选框勾选状态
            ///</summary>
            ///<param name="checked" type="bool">复选框勾选状态.</param>
            var checkbox = this, options = this.Options;
            var disabled = $(checkbox.Element).attr("disabled");
            if (disabled)
                return;
            checkbox.Set({ Checked: checked });
            $(checkbox.Element).trigger("change");
        },
        Toggle: function () {
            ///<summary>
            /// 设置复选框勾选状态
            ///</summary>
            var checkbox = this, options = this.Options;
            var checked = $(checkbox.Element)[0].checked;
            checkbox.Checked(!checked);
        }
    });
})(jQuery);

/**
* jQuery FrameUI 2.1.0
* 
* 作者：zlx
* 日期：2015.06.20
* 说明：Combobox 控件。
*/
(function ($) {
    $.fn.ComboBox = function (options) {
        return $.Frame.Do.call(this, "ComboBox", arguments);
    };

    $.Methods.ComboBox = $.Methods.ComboBox || {};

    $.Defaults.ComboBox = {
        Editable: true,
        Width: 100,
        PanelWidth: null,
        PanelHeight: null,
        Mode: "primary",
        //Resize: true,           //下拉列表面板是否允许调整大小
        Value: "Key",
        Text: "Value",
        //ApplyToolTip: false,    // 该属性用于当下拉框选项文本超出长度以省略号隐藏时，鼠标移动到下拉框上方时以提示信息显示全文
        DefaultText: "请选择",
        Datas: null,            // [{ Key,Value }]
        Url: null,              //数据源URL(需返回JSON)
        Method: "POST",
        DataType: "json",
        Params: null,
        Disabled: false,
        OnBeforeSelect: false, //选择前事件
        OnSelected: null, //选择值事件 
        OnSuccess: null,
        OnError: null,
        OnComplete: null,
        OnResolve: null,
        OnBeforeOpen: null      //打开下拉框前事件，可以通过return false来阻止继续操作，利用这个参数可以用来调用其他函数，比如打开一个新窗口来选择值
    };

    $.Frame.Controls.ComboBox = function (element, options) {
        $.Frame.Controls.ComboBox.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.ComboBox.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "ComboBox";
        },
        _IdPrev: function () {
            return "ComboBoxFor";
        },
        _ExtendMethods: function () {
            return $.Methods.ComboBox;
        },
        _PreRender: function () {
            var combobox = this, options = this.Options;
            if (!$(combobox.Element).hasClass("frame-combobox")) {
                $(combobox.Element).addClass("frame-combobox");
            }

            // 这里Selection变量主要用来记录当前下拉框所选选项信息
            combobox.Selection = { Text: options.DefaultText, Value: "-1" };
        },
        _Render: function () {
            var combobox = this, options = this.Options;

            combobox.Container = $("<div></div>").addClass("frame-combobox-container")
                                    .on("click", function () { combobox.ToggleDropDown(); });
            combobox.Inner = $("<div></div>").addClass("frame-combobox-inner");
            combobox.SelectionPanel = $("<div></div>").addClass("frame-combobox-selection-panel");
            combobox.Menu = $("<ul></ul>").addClass("frame-combobox-menu").appendTo(combobox.SelectionPanel);

            $(combobox.Element).wrap(combobox.Inner);
            $(combobox.Element).parent().wrap(combobox.Container);
            $(combobox.Element).parent().after(combobox.SelectionPanel);

            combobox.Set(options);
        },
        _SetEditable: function (editalbe) {
            var combobox = this, options = this.Options;
            if (editalbe) {
                var input = $('<input type="text" />').addClass("frame-combobox-text");
                $(combobox.Element).parent().append(input);
            } else {
                var div = $("<div></div>").addClass("frame-combobox-text");
                $(combobox.Element).parent().append(div);
            }

            var angle = $("<i></i>").addClass("icon-sort-down");
            $(combobox.Element).parent().append(angle);
        },
        _SetWidth: function (width) {
            var combobox = this, options = this.Options;
            if (width) {
                var angleWidth = $("i.icon-sort-down", $(combobox.Element).parent()).outerWidth(true);
                if (options.Editable)
                    $("input.frame-combobox-text", $(combobox.Element).parent()).width(width - angleWidth - 10);
                else
                    $("div.frame-combobox-text", $(combobox.Element).parent()).width(width - angleWidth - 10);
            }
        },
        _SetPanelWidth: function (width) {
            var combobox = this, options = this.Options;
            if (width) {
                combobox.Menu.width(width);
            }
            else {
                combobox.Menu.width($(combobox.Element).parent().width());
            }
        },
        _SetPanelHeight: function (height) {
            var combobox = this, options = this.Options;
            if (height) {
                combobox.SelectionPanel.height(height);
            }
        },
        _SetMode: function (mode) {
            var combobox = this, options = this.Options;
            if (mode)
                combobox.Menu.addClass("frame-combobox-" + mode);
        },
        _SetDisabled: function (disabled) {
            var combobox = this, options = this.Options;

            if (disabled) {
                $(combobox.Element).parent().addClass("disabled");
                if (options.Editable)
                    $("input.frame-combobox-text", $(combobox.Element).parent()).attr("disabled", true);
            } else {
                $(combobox.Element).parent().removeClass("disabled");
                if (options.Editable)
                    $("input.frame-combobox-text", $(combobox.Element).parent()).removeAttr("disabled");

            }
        },
        _SetDatas: function (datas) {
            var combobox = this, options = this.Options;

            var index = 0;
            if (options.DefaultText) {
                combobox.Menu.append(combobox._CreateListItem(index, -1, options.DefaultText));
            }

            if ($(combobox.Element).children().length > 0) {
                $(combobox.Element).children("option").each(function (idx) {
                    index = index + 1;
                    combobox.Menu.append(combobox._CreateListItem(index, this.value, this.text));
                });
            }

            if (datas) {
                if (combobox.HasBind("Resolve"))
                    datas = combobox.Trigger("Resolve", [datas]);
                $(datas).each(function (idx) {
                    index = index + 1;
                    combobox.Menu.append(combobox._CreateListItem(index, this[options.Value], this[options.Text]));
                });
            }

            combobox.Select(0);
        },
        _CreateListItem: function (idx, value, text) {
            var combobox = this;
            var item = $("<li></li>").attr("idx", idx).on("click", function () {
                var index = parseInt($(this).attr("idx"));
                combobox.Select(index);
                return false;
            });
            var link = $("<a></a>").attr("href", "#").attr("val", value).text(text).appendTo(item);
            return item;
        },
        _SetUrl: function (url) {
            var combobox = this, options = this.Options;
            if (url != null) {
                combobox.Menu.empty();
                $.ajax({
                    dataType: options.DataType, type: options.Method,
                    url: url,
                    data: options.Params,
                    success: function (data) {
                        if (combobox.HasBind("Success"))
                            data = combobox.Trigger("Success", [data]);
                        combobox.Set({ Datas: data });
                    },
                    error: function (xhr, info, exp) {
                        // 三个参数:XMLHttpRequest对象、错误信息、(可选)捕获的异常对象
                        if (combobox.HasBind("Error"))
                            combobox.Trigger("Error", [{ XHR: xhr, Info: info, Exp: exp}]);
                    },
                    complete: function (xhr, ts) {
                        if (combobox.HasBind("Complete"))
                            combobox.Trigger("Complete", [{ XHR: xhr, TS: ts}]);
                        combobox.Select(0);
                    }
                });
            }
        },
        Disable: function () {
            // 禁用控件
            var combobox = this, options = this.Options;
            combobox.Set({ Disabled: true });
        },
        Enable: function () {
            // 启用控件
            var combobox = this, options = this.Options;
            combobox.Set({ Disabled: false });
        },
        Select: function (which) {
            ///<summary>
            /// 选择指定的选项
            ///</summary>
            ///<param name="which" type="which">选项的索引值或名称.当参数为索引值时,起始值从0开始.</param>
            var combobox = this, options = this.Options;

            var pass = true;
            if (combobox.HasBind("BeforeSelect"))
                pass = combobox.Trigger("BeforeSelect", []);

            if (pass) {
                var listitem = null;
                var item = null;
                if (typeof which == "number") {
                    $("li.active", combobox.Menu).removeClass("active");
                    listitem = combobox.Menu.children().eq(which).addClass("active");
                    item = $("a", listitem);
                    combobox.Selection = { Text: item.text(), Value: item.attr("val") };
                } else {
                    for (var i = 0, count = combobox.Menu.children().length; i < count; i++) {
                        listitem = combobox.Menu.children().eq(i);
                        item = $("a", listitem);
                        if (item.text() == which) {
                            $("li.active", combobox.Menu).removeClass("active");
                            listitem.addClass("active");
                            combobox.Selection = { Text: item.text(), Value: item.attr("val") };
                        }
                    }
                }

                if (options.Editable)
                    $("input.frame-combobox-text", $(combobox.Element).parent()).val(combobox.Selection.Text);
                else
                    $("div.frame-combobox-text", $(combobox.Element).parent()).text(combobox.Selection.Text);

                if (combobox.HasBind("Selected"))
                    combobox.Trigger("Selected", [listitem, combobox.Selection]);
            }

            combobox.SelectionPanel.slideUp("fast");
            return combobox.Selection;
        },
        ToggleDropDown: function () {
            var combobox = this, options = this.Options;
            var canOpen = true;
            if (!combobox.SelectionPanel.is(":visible") && combobox.HasBind("BeforeOpen"))
                canOpen = combobox.Trigger("BeforeOpen", []);

            if (!$(combobox.Element).parent().hasClass("disabled") && canOpen)
                combobox.SelectionPanel.slideToggle("fast");
        },
        GetText: function () {
            ///<summary>
            /// 获取当前选中的选项文本
            ///</summary>
            var combobox = this, options = this.Options;
            return combobox.Selection.Text;
        },
        SetText: function (text) {
            ///<summary>
            /// 设置当前选项文本
            ///</summary>
            ///<param name="text" type="string">要选中的选项文本</param>
            var combobox = this, options = this.Options;
            combobox.Select(text);
        },
        GetValue: function () {
            ///<summary>
            /// 获取当前选中的选项值
            ///</summary>
            var combobox = this, options = this.Options;
            return combobox.Selection.Value;
        },
        SetValue: function (value) {
            ///<summary>
            /// 设置当前选项值
            ///</summary>
            ///<param name="value" type="string">要选中的选项值</param>
            var combobox = this, options = this.Options;
            for (var i = 0, count = combobox.Menu.children().length; i < count; i++) {
                var listitem = combobox.Menu.children().eq(i);
                var item = $("a", listitem);
                if (item.attr("val") == value) {
                    $("li.active", combobox.Menu).removeClass("active");
                    listitem.addClass("active");
                    combobox.Selection = { Text: item.text(), Value: item.attr("val") };
                }
            }

            if (options.Editable)
                $("input.frame-combobox-text", $(combobox.Element).parent()).val(combobox.Selection.Text);
            else
                $("div.frame-combobox-text", $(combobox.Element).parent()).text(combobox.Selection.Text);
        },
        Clear: function () {
            ///<summary>
            /// 重置下拉框
            ///</summary>
            var combobox = this, options = this.Options;
            combobox.Selection = { Text: options.DefaultText, Value: "-1" };
            combobox.Select(0);
        },
        Reload: function (params) {
            ///<summary>
            /// 重新进行远程请求数据并绑定到下拉框
            ///</summary>
            ///<param name="params" type="object">请求远程数据参数</param>
            var combobox = this, options = this.Options;
            options.Params = params;
            combobox.Set({ Url: options.Url });
        },
        LoadData: function (datas) {
            ///<summary>
            /// 加载数据并绑定到下拉框
            ///</summary>
            ///<param name="datas" type="array">要进行加载绑定的数据集合</param>
            var combobox = this, options = this.Options;
            combobox.Menu.empty();
            combobox.Set({ Datas: datas });
            combobox.Select(0);
        }
    });
})(jQuery);

/**
* jQuery FrameUI 2.1.0
* 
* 作者：zlx
* 日期：2015.06.20
* 说明：ContextMenu 控件。
*/
(function ($) {
    $.ContextMenu = function (options) {
        return $.Frame.Do.call(null, "ContextMenu", arguments, { IsStatic: true });
    };

    $.fn.ContextMenu = function (options) {
        options = $.extend({}, $.Defaults.ContextMenu, options || {});

        options.Target = options.Target || $(this);

        this.each(function () {
            var id = $(this).attr("dependOn");
            if (!id) {
                $.ContextMenu(options);
            }

            $(this).on("contextmenu", function (e) {
                return false;
            }).on("mousedown", function (e) {
                if (3 == e.which) {
                    var contextmenu = $.Frame.Get($(this).attr("dependOn"));
                    if (!contextmenu)
                        return;
                    contextmenu.Show({
                        X: (e.offsetX || e.clientX - $(e.target).offset().left + window.pageXOffset),
                        Y: (e.offsetY || e.clientY - $(e.target).offset().top + window.pageYOffset)
                    });
                }
            }).on("click", function (e) {
                var contextmenu = $.Frame.Get($(this).attr("dependOn"));
                if (!contextmenu)
                    return;
                contextmenu.Close();
            });
        });

        return $.Frame.Get(this, "dependOn");
    };

    $.Methods.ContextMenu = $.Methods.ContextMenu || {};

    $.Defaults.ContextMenu = {
        Target: null,
        Width: null,
        Mode: "primary",
        Items: [],    // {Text,HasChild,Childs:[],Icon,IconColor,Divider,Disabled,Actived,OnClick}
        X: 0,
        Y: 0,
        OnItemClick: null       // 该事件若 return false，则会忽略最前端的click隐藏菜单事件
    };

    $.Frame.Controls.ContextMenu = function (options) {
        $.Frame.Controls.ContextMenu.Parent.constructor.call(this, null, options);
    };

    $.Frame.Controls.ContextMenu.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "ContextMenu";
        },
        _IdPrev: function () {
            return "ContextMenuFor";
        },
        _ExtendMethods: function () {
            return $.Methods.ContextMenu;
        },
        _Render: function () {
            var contextmenu = this, options = this.Options;

            contextmenu.Target = options.Target;
            contextmenu.Target.attr("dependOn", contextmenu.Id);

            contextmenu.Container = $("<div></div>").addClass("frame-dropdown")
                                                    .addClass("frame-dropdown-preview")
                                                    .appendTo(contextmenu.Target).hide();
            contextmenu.Menu = $("<ul></ul>").addClass("frame-dropdown-menu").addClass("frame-dropdown-" + options.Mode).appendTo(contextmenu.Container);

            contextmenu.Set(options);
        },
        _SetWidth: function (width) {
            var contextmenu = this, options = this.Options;
            if (width)
                contextmenu.Menu.width(width);
        },
        _SetX: function (x) {
            var contextmenu = this, options = this.Options;
            $(contextmenu.Container).css("left", x);
        },
        _SetY: function (y) {
            var contextmenu = this, options = this.Options;
            $(contextmenu.Container).css("top", y);

        },
        _SetItems: function (items) {
            ///<summary>
            /// 设置下拉菜单的菜单项
            ///</summary>
            ///<param name="items" type="array">菜单项对象集合.
            /// 菜单项对象结构[{Text,HasChild,Childs:[],Icon,IconColor,Divider,Disabled,Actived,OnClick}]</param>
            var contextmenu = this, options = this.Options;

            $(items).each(function (idx) {
                contextmenu._Visitor(1, idx, this, contextmenu.Menu);
            });
        },
        _Visitor: function (lv, idx, obj, parent) {
            ///<summary>
            /// 生成可访问的菜单项
            ///</summary>
            ///<param name="lv" type="number">菜单项所属的层级.层级数从1起始.</param>
            ///<param name="idx" type="number">菜单项在集合中的索引.索引值从0起始.</param>
            ///<param name="obj" type="object">菜单项对象</param>
            ///<param name="parent" type="selector">装载菜单项元素的元素容器</param>
            var contextmenu = this, options = this.Options;
            var item = $("<li></li>").appendTo(parent).attr("lv", "lv_" + lv + "_" + idx);
            if (obj.OnClick) {
                item.on("click", function () {
                    if (item.hasClass("disabled"))
                        return false;
                    return obj.OnClick.call(this, this, obj);
                });
            } else if (contextmenu.HasBind("ItemClick") && !obj.HasChild) {
                item.on("click", function () {
                    if (item.hasClass("disabled"))
                        return false;
                    return contextmenu.Trigger("ItemClick", [this, obj]);
                });
            }
            if (!obj.Divider) {
                if (obj.Disabled)
                    item.addClass("disabled");
                else if (obj.Actived && !obj.Disabled)
                    item.addClass("active");
                if (obj.HasChild) {
                    item.addClass("frame-dropdown-submenu");
                    contextmenu._AppendTermWithChild(item, obj.Icon, obj.IconColor, obj.Text);
                    var subMenu = $("<ul></ul>").addClass("frame-dropdown-menu")
                                                .addClass("frame-dropdown-" + options.Mode)
                                                .addClass("pull-right").appendTo(item);
                    $(obj.Childs).each(function (i) {
                        contextmenu._Visitor((lv + 1), i, this, subMenu);
                    });
                } else {
                    contextmenu._AppendTermNonChild(item, obj.Icon, obj.IconColor, obj.Text);
                    return;
                }
            } else {
                item.addClass("divider");
                return;
            }
        },
        _AppendTermNonChild: function (item, icon, color, text) {
            var term = $("<a></a>").attr("href", "#").appendTo(item);
            if (icon)
                $("<i></i>").addClass(icon).addClass(color).css("margin-right", "0.3em").appendTo(term);
            term.append(text);
        },
        _AppendTermWithChild: function (item, icon, color, text) {
            var term = $("<a></a>").attr("href", "#").addClass("clearfix").appendTo(item);
            if (icon)
                $("<i></i>").addClass(icon).addClass("pull-left").addClass(color).appendTo(term);
            term.append($("<span></span>").addClass("pull-left").text(text));
            term.append($("<i></i>").addClass("icon-caret-right").addClass("pull-right"));
        },
        Show: function (option) {
            var contextmenu = this;
            contextmenu.Set(option);
            contextmenu.Container.slideDown();
        },
        Close: function () {
            var contextmenu = this;
            contextmenu.Container.slideUp();
        },
        EnableItem: function (lv, idx) {
            var contextmenu = this, options = this.Options;
            $(contextmenu.Menu).find("li[lv='lv_" + lv + "_" + idx + "']").removeClass("disabled");
        },
        DisableItem: function (lv, idx) {
            var contextmenu = this, options = this.Options;
            $(contextmenu.Menu).find("li[lv='lv_" + lv + "_" + idx + "']").addClass("disabled");
        },
        DeactiveItem: function (lv, idx) {
            var contextmenu = this, options = this.Options;
            $(contextmenu.Menu).find("li[lv='lv_" + lv + "_" + idx + "']").removeClass("active");
        },
        ActiveItem: function (lv, idx) {
            var contextmenu = this, options = this.Options;
            $(contextmenu.Menu).find("li[lv='lv_" + lv + "_" + idx + "']").addClass("active");
        }
    });

})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.13
* 说明：DatePicker 日期控件。
*/
(function ($) {
    $.fn.DatePicker = function (options) {
        return $.Frame.Do.call(this, "DatePicker", arguments);
    };

    $.Methods.DatePicker = $.Methods.DatePicker || {};

    $.Defaults.DatePicker = {
        Width: 165,
        ShowType: "ymd", // 显示控件类型：ymd,ym
        DateFormat: "yyyy-MM-dd hh:mm", //在日期文本框中显示的日期格式 yyyy-MM-dd hh:mm/yyyy-MM-dd等
        IsShowDate: true, //是否默认显示日期
        CanShowTime: true, // 是否显示出时间
        Disabled: false,
        ReadOnly: false,
        OnExtractor: null,  // 对日期值作进一步格式处理,参数为date(string)
        OnChange: null,
        OnResolve: null     // 对设置传入的日期值进行格式反转，以使其符合内部运行格式
    };

    $.Frame.Controls.DatePicker = function (element, options) {
        $.Frame.Controls.DatePicker.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.DatePicker.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "DatePicker";
        },
        _IdPrev: function () {
            return "DatePickerFor";
        },
        _ExtendMethods: function () {
            return $.Methods.DatePicker;
        },
        _PreRender: function () {
            var datepicker = this;

            if (!$(datepicker.Element).hasClass("frame-datepicker")) {
                $(datepicker.Element).addClass("frame-datepicker");
            }

            datepicker.Calendar = null;
            datepicker.CalendarId = "dp_" + datepicker.Id;

            var date = new Date();

            // 当前日期
            datepicker.Now = {
                Year: date.getFullYear().toString(),
                Month: (date.getMonth() + 1) < 10 ? ("0" + (date.getMonth() + 1)) : (date.getMonth() + 1).toString(),
                Day: date.getDate() < 10 ? "0" + date.getDate() : date.getDate().toString(),
                Hour: date.getHours() < 10 ? "0" + date.getHours() : date.getHours().toString(),
                Minute: date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes().toString(),
                PrevYear: function (value) {
                    this.Year = (parseInt(this.Year) - (value ? value : 1)).toString();
                    return this;
                },
                PrevMonth: function () {
                    var month = parseInt(this.Month) - 1;
                    if (month == 0) {
                        month = 12;
                        this.PrevYear();
                    }
                    this.Month = month < 10 ? "0" + month : month.toString();
                    return this;
                },
                NextMonth: function () {
                    var month = parseInt(this.Month) + 1;
                    if (month > 12) {
                        month = 1;
                        this.NextYear();
                    }
                    this.Month = month < 10 ? "0" + month : month.toString();
                    return this;
                },
                NextYear: function (value) {
                    this.Year = (parseInt(this.Year) + (value ? value : 1)).toString();
                    return this;
                },
                ToDateTime: function () {
                    var date = null;
                    if (datepicker.Options.CanShowTime)
                        date = new Date(this.Year + "/" + this.Month + "/" + this.Day + " " + this.Hour + ":" + this.Minute);
                    else
                        date = new Date(this.Year + "/" + this.Month + "/" + this.Day);

                    return date;
                }
            };

            datepicker._PageYear = parseInt(datepicker.Now.Year.substring(0, 2) + "09") + 10;
            datepicker.ActiveDate = {
                Year: datepicker.Now.Year,
                Month: datepicker.Now.Month,
                Day: datepicker.Now.Day,
                Hour: datepicker.Now.Hour,
                Minute: datepicker.Now.Minute,
                PrevYear: function (value) {
                    this.Year = (parseInt(this.Year) - (value ? value : 1)).toString();
                    return this;
                },
                PrevMonth: function () {
                    var month = parseInt(this.Month) - 1;
                    if (month == 0) {
                        month = 12;
                        this.PrevYear();
                    }
                    this.Month = month < 10 ? "0" + month : month.toString();
                    return this;
                },
                NextMonth: function () {
                    var month = parseInt(this.Month) + 1;
                    if (month > 12) {
                        month = 1;
                        this.NextYear();
                    }
                    this.Month = month < 10 ? "0" + month : month.toString();
                    return this;
                },
                NextYear: function (value) {
                    this.Year = (parseInt(this.Year) + (value ? value : 1)).toString();
                    return this;
                },
                ToDateTime: function () {
                    var date = null;
                    if (datepicker.Options.CanShowTime)
                        date = new Date(this.Year + "/" + this.Month + "/" + this.Day + " " + this.Hour + ":" + this.Minute);
                    else
                        date = new Date(this.Year + "/" + this.Month + "/" + this.Day);

                    return date;
                }
            };
        },
        _Render: function () {
            var datepicker = this, options = this.Options;

            datepicker.Container = $("<div></div>").addClass("frame-datepicker-container");
            $(datepicker.Element).wrap(datepicker.Container);
            datepicker.AddonButton = $("<span></span>").addClass("frame-datepicker-addon")
                                                       .append('<i class="icon-calendar bigger-110"></i>')
                                                       .on("click", function () {
                                                           if (datepicker.Disabled)
                                                               return false;
                                                           $(datepicker.Element).focus();
                                                           datepicker._ResetActiveDate();
                                                           datepicker.Set(options);
                                                           datepicker._OnCalendarResize();
                                                           $("#" + datepicker.CalendarId).slideToggle("fast");
                                                           return false;
                                                       });
            $(datepicker.Element).after(datepicker.AddonButton);
            $(datepicker.Element).on("keyup.DatePicker change.DatePicker", function (e) {
                if (datepicker.HasBind("Change"))
                    datepicker.Trigger("Change", [this.value]);
                var v = datepicker._Resolve(this.value);
                if (v != null) {
                    datepicker._ResetActiveDate();
                    datepicker.Set(options);
                }
                return false;
            });

            datepicker._CreateCalendar();
            datepicker.Set(options);
        },
        _ResetActiveDate: function () {
            var datepicker = this, options = this.Options;
            datepicker.ActiveDate = $.extend(datepicker.ActiveDate, datepicker.Now);
            datepicker._PageYear = parseInt(datepicker.Now.Year.substring(0, 2) + "09") + 10;
        },
        _CreateCalendar: function () {
            var datepicker = this, options = this.Options;

            datepicker.Calendar = $("<div></div>").addClass("frame-datepicker-panel")
                                                  .attr("id", datepicker.CalendarId).appendTo("body");

            datepicker._CreateCalendarForDays();
            datepicker._CreateCalendarForMonths();
            datepicker._CreateCalendarForYears();
            datepicker._CreateCalendarForHours();
            datepicker._CreateCalendarForMinutes();

            datepicker._CreateCalendarFooter();
        },
        _CreateCalendarFooter: function () {
            var datepicker = this, options = this.Options;

            datepicker.Calendar.append($("<div></div>").addClass("frame-datepicker-panel-foot"));
            datepicker.Calendar.TimeContainer = $("<div></div>").addClass("frame-datepicker-panel-time").hide();

            $(".frame-datepicker-panel-foot", datepicker.Calendar).append(datepicker.Calendar.TimeContainer);
            $("<label></label>").addClass("frame-label").addClass("frame-label-transparent").addClass("input-mini")
                                .addClass("for-hour").text(datepicker.Now.Hour)
                                .appendTo(datepicker.Calendar.TimeContainer)
                                .on("click", function () {
                                    if (!$(".frame-datepicker-hours", datepicker.Calendar).is(":visible")) {
                                        $(".frame-datepicker-days", datepicker.Calendar).hide();
                                        $(".frame-datepicker-minutes", datepicker.Calendar).hide();
                                        $(".frame-datepicker-hours", datepicker.Calendar).show();
                                        datepicker._BuildCalendarContentForHours();
                                    } else {
                                        $(".frame-datepicker-days", datepicker.Calendar).show();
                                        $(".frame-datepicker-hours", datepicker.Calendar).hide();
                                    }
                                });
            datepicker.Calendar.TimeContainer.append("<span>:</span>");
            $("<label></label>").addClass("frame-label").addClass("frame-label-transparent").addClass("input-mini")
                                .addClass("for-minute").text(datepicker.Now.Minute)
                                .appendTo(datepicker.Calendar.TimeContainer)
                                .on("click", function () {
                                    if (!$(".frame-datepicker-minutes", datepicker.Calendar).is(":visible")) {
                                        $(".frame-datepicker-days", datepicker.Calendar).hide();
                                        $(".frame-datepicker-hours", datepicker.Calendar).hide();
                                        $(".frame-datepicker-minutes", datepicker.Calendar).show();
                                        datepicker._BuildCalendarContentForMinutes();
                                    } else {
                                        $(".frame-datepicker-days", datepicker.Calendar).show();
                                        $(".frame-datepicker-minutes", datepicker.Calendar).hide();
                                    }
                                });
            $(".frame-datepicker-panel-foot", datepicker.Calendar).append($('<span class="today">关闭</span>')
                                .on("click", function () {
                                    $("#" + datepicker.CalendarId).slideUp("fast");
                                    return false;
                                }));
        },
        _GetWeekOnFirstDay: function () {
            ///<summary>
            /// 获取指定日期(默认当前日期)当月第一天对应的星期
            /// 0:周日,1:周一,2:周二,3:周三,4:周四,5:周五,6:周六
            ///</summary>
            var datepicker = this, options = this.Options;
            var dateString = datepicker.ActiveDate.ToDateTime().Format("yyyy/MM/1");
            return new Date(dateString).getDay();
        },
        _GetDaysOfDate: function () {
            ///<summary>
            /// 获取当前日期月份的天数
            ///</summary>
            var datepicker = this, options = this.Options;
            //var dateString = datepicker.ActiveDate.ToDateTime().AddMonth(1).Format("yyyy/MM/0");
            var year=parseInt(datepicker.ActiveDate.Year);
            var month=parseInt(datepicker.ActiveDate.Month);
            return new Date(year,month,0).getDate();
        },
        _GetLastDaysOfDate: function () {
            ///<summary>
            /// 获取当前日期的上一个月天数
            ///</summary>
            var datepicker = this, options = this.Options;
            //var dateString = datepicker.ActiveDate.ToDateTime().Format("yyyy/MM/0");
            var year=parseInt(datepicker.ActiveDate.Year);
            var month=parseInt(datepicker.ActiveDate.ToDateTime().AddMonth(-1).getMonth()+1);
            return new Date(year,month,0).getDate();
        },
        _SetHeadText: function (table) {
            var datepicker = this, options = this.Options;

            var headtext = datepicker.ActiveDate.Year + "年" + datepicker.ActiveDate.Month + "月";
            $("thead tr", table).first().children(".switch").text(headtext);
        },
        _CreateCalendarForDays: function () {
            var datepicker = this, options = this.Options;
            var container = $("<div></div>").addClass("frame-datepicker-days").appendTo(datepicker.Calendar);
            var table = $("<table></table>").addClass("table-condensed").append("<thead></thead><tbody></tbody>")
                        .appendTo(container);
            $("thead", table).append("<tr></tr>");
            $("thead > tr", table).append($("<th></th>").addClass("prev").css("visibility", "visible").append($("<i></i>").addClass("icon-arrow-left"))
                                                        .on("click", function () {
                                                            datepicker.ActiveDate.PrevMonth();
                                                            datepicker._BuildCalendarContentForDays();
                                                        }));
            $("thead > tr", table).append($('<th colspan="5"></th>').addClass("switch")
                                                                    .on("click", function () {
                                                                        datepicker._SetCanShowTime(false);
                                                                        $(".frame-datepicker-days", datepicker.Calendar).hide();
                                                                        $(".frame-datepicker-months", datepicker.Calendar).show();
                                                                        datepicker._BuildCalendarContentForMonths();
                                                                    }));
            $("thead > tr", table).append($("<th></th>").addClass("next").css("visibility", "visible").append($("<i></i>").addClass("icon-arrow-right"))
                                                        .on("click", function () {
                                                            datepicker.ActiveDate.NextMonth();
                                                            datepicker._BuildCalendarContentForDays();
                                                        }));

            $("thead", table).append("<tr></tr>");
            $("thead tr:last", table).append($("<th>日</th>").addClass("dow"))
                                                   .append($("<th>一</th>").addClass("dow"))
                                                   .append($("<th>二</th>").addClass("dow"))
                                                   .append($("<th>三</th>").addClass("dow"))
                                                   .append($("<th>四</th>").addClass("dow"))
                                                   .append($("<th>五</th>").addClass("dow"))
                                                   .append($("<th>六</th>").addClass("dow"));

            $("tbody", table).append($("<tr></tr>").append('<td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td>'));
            $("tbody", table).append($("<tr></tr>").append('<td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td>'));
            $("tbody", table).append($("<tr></tr>").append('<td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td>'));
            $("tbody", table).append($("<tr></tr>").append('<td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td>'));
            $("tbody", table).append($("<tr></tr>").append('<td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td>'));
            $("tbody", table).append($("<tr></tr>").append('<td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td>'));

        },
        _BuildCalendarContentForDays: function () {
            var datepicker = this, options = this.Options;

            datepicker.Set({ CanShowTime: options.CanShowTime });
            // 当前日期第一天星期
            var week = datepicker._GetWeekOnFirstDay();
            // 当前日期天数
            var days = datepicker._GetDaysOfDate();
            // 上一个月天数
            var lastDays = datepicker._GetLastDaysOfDate();

            var table = $(".frame-datepicker-days > .table-condensed", datepicker.Calendar);
            datepicker._SetHeadText(table);

            $("tbody td.active", table).removeClass("active");
            $("tbody td", table).off("click");
            $("tbody", table).children().each(function (idx) {
                var day;
                if (idx == 0 && week > 0) {
                    for (var index = 1; index <= week; index++) {
                        day = lastDays - (week - index);
                        $(this).children().eq(index - 1).text(day).addClass("old")
                            .on("click", function () {
                                datepicker.ActiveDate.PrevMonth();
                                datepicker.Now.Month = datepicker.ActiveDate.Month;
                                datepicker.ActiveDate.Day = parseInt($(this).text()) < 10 ? "0" + $(this).text() : $(this).text();
                                datepicker.Now.Day = datepicker.ActiveDate.Day;
                                datepicker._SetDate();
                                $("#" + datepicker.CalendarId).slideUp("fast");
                                datepicker._BuildCalendarContentForDays();
                                return false;
                            });
                    }
                }
                for (var i = (idx == 0 ? week : 0); i < 7; i++) {
                    day = (idx * 7) + (i - week) + 1;
                    var toggle = (day <= days);
                    $(this).children().eq(i).text(toggle ? day : day - days).removeClass("old").toggleClass("new", !toggle)
                            .on("click", function () {
                                if ($(this).hasClass("new")) {
                                    datepicker.ActiveDate.NextMonth();
                                    datepicker.Now.Month = datepicker.ActiveDate.Month;
                                }

                                datepicker.Now.Year = datepicker.ActiveDate.Year;
                                datepicker.Now.Month = datepicker.ActiveDate.Month;
                                datepicker.Now.Hour = datepicker.ActiveDate.Hour;
                                datepicker.Now.Minute = datepicker.ActiveDate.Minute;
                                datepicker.Now.Day = parseInt($(this).text()) < 10 ? "0" + $(this).text() : $(this).text();
                                datepicker.ActiveDate.Day = datepicker.Now.Day;
                                datepicker._SetDate();
                                $("#" + datepicker.CalendarId).slideUp("fast");
                                datepicker._BuildCalendarContentForDays();
                                return false;
                            });
                    if (datepicker.Now.Year == datepicker.ActiveDate.Year &&
                    datepicker.Now.Month == datepicker.ActiveDate.Month &&
                    day == parseInt(datepicker.Now.Day)) {
                        $(this).children().eq(i).addClass("active");
                    }
                }
            });
        },
        _CreateCalendarForMonths: function () {
            var datepicker = this, options = this.Options;

            var container = $("<div></div>").addClass("frame-datepicker-months").appendTo(datepicker.Calendar);
            var table = $("<table></table>").addClass("table-condensed").append("<thead></thead><tbody></tbody>")
                        .appendTo(container);

            $("thead", table).append("<tr></tr>");
            $("thead > tr", table).append($("<th></th>").addClass("prev").css("visibility", "hidden").append($("<i></i>").addClass("icon-arrow-left")));
            $("thead > tr", table).append($('<th colspan="5"></th>').addClass("switch")
                                                                    .on("click", function () {
                                                                        $(".frame-datepicker-months", datepicker.Calendar).hide();
                                                                        $(".frame-datepicker-years", datepicker.Calendar).show();
                                                                        datepicker._BuildCalendarContentForYears();
                                                                    }));
            $("thead > tr", table).append($("<th></th>").addClass("next").css("visibility", "hidden").append($("<i></i>").addClass("icon-arrow-right")));

            $("tbody", table).append("<tr></tr>");
            $("tbody > tr", table).append('<td colspan="7"><span class="month">1月</span><span class="month">2月</span>'
                + '<span class="month">3月</span><span class="month">4月</span><span class="month">5月</span>'
                + '<span class="month">6月</span><span class="month">7月</span><span class="month">8月</span>'
                + '<span class="month">9月</span><span class="month">10月</span><span class="month">11月</span>'
                + '<span class="month">12月</span></td>');

            datepicker._BuildCalendarContentForMonths();
        },
        _BuildCalendarContentForMonths: function () {
            var datepicker = this, options = this.Options;

            var table = $(".frame-datepicker-months > .table-condensed", datepicker.Calendar);

            var headtext = datepicker.ActiveDate.Year + "年";
            $("thead tr", table).first().children(".switch").text(headtext);

            $("tbody > tr > td span.active", table).removeClass("active");

            var mstring = parseInt(datepicker.Now.Month) + "月";
            $("tbody > tr > td", table).children().each(function () {
                if (datepicker.Now.Year == datepicker.ActiveDate.Year && $(this).text() == mstring)
                    $(this).addClass("active");
            }).on("click", function () {
                var v = parseInt($(this).text().match(/[1-9][0-9]*/g)[0]);
                if (options.ShowType == "ymd") {
                    datepicker.ActiveDate.Month = v < 10 ? "0" + v : v;
                    datepicker._BuildCalendarContentForDays();
                    $(".frame-datepicker-months", datepicker.Calendar).hide();
                    $(".frame-datepicker-days", datepicker.Calendar).show();
                }
                else {
                    datepicker.Now.Month = v < 10 ? "0" + v : v;
                    datepicker._SetDate();
                    $("#" + datepicker.CalendarId).slideUp("fast");
                }
                return false;
            });
        },
        _CreateCalendarForYears: function () {
            var datepicker = this, options = this.Options;

            var container = $("<div></div>").addClass("frame-datepicker-years").appendTo(datepicker.Calendar);
            var table = $("<table></table>").addClass("table-condensed").append("<thead></thead><tbody></tbody>")
                        .appendTo(container);

            $("thead", table).append("<tr></tr>");
            $("thead > tr", table).append($("<th></th>").addClass("prev").css("visibility", "visible").append($("<i></i>").addClass("icon-arrow-left"))
                                                        .on("click", function () {
                                                            datepicker._PageYear -= 10;
                                                            datepicker._BuildCalendarContentForYears();
                                                        }));
            $("thead > tr", table).append($('<th colspan="5"></th>').addClass("switch"));
            $("thead > tr", table).append($("<th></th>").addClass("next").css("visibility", "visible").append($("<i></i>").addClass("icon-arrow-right"))
                                                        .on("click", function () {
                                                            datepicker._PageYear += 10;
                                                            datepicker._BuildCalendarContentForYears();
                                                        }));

            $("tbody", table).append("<tr></tr>");
            $("tbody > tr", table).append('<td colspan="7"><span class="year"></span><span class="year"></span>'
                + '<span class="year"></span><span class="year"></span><span class="year"></span>'
                + '<span class="year"></span><span class="year"></span><span class="year"></span>'
                + '<span class="year"></span><span class="year"></span><span class="year"></span>'
                + '<span class="year"></span></td>');

            datepicker._BuildCalendarContentForYears();
        },
        _BuildCalendarContentForYears: function () {
            var datepicker = this, options = this.Options;

            var year = datepicker._PageYear;
            var start = year - 10;

            var table = $(".frame-datepicker-years > .table-condensed", datepicker.Calendar);

            var headtext = start + "-" + year;
            $("thead tr", table).first().children(".switch").text(headtext);
            $("tbody > tr > td span.active", table).removeClass("active");

            $("tbody > tr > td", table).children().each(function (i) {
                $(this).text(start + i);
                if ((start + i) == datepicker.Now.Year)
                    $(this).addClass("active");
            }).on("click", function () {
                datepicker.ActiveDate.Year = $(this).text();
                $(".frame-datepicker-months", datepicker.Calendar).show();
                $(".frame-datepicker-years", datepicker.Calendar).hide();
                datepicker._BuildCalendarContentForMonths();
            });
        },
        _CreateCalendarForHours: function () {
            var datepicker = this, options = this.Options;
            var container = $("<div></div>").addClass("frame-datepicker-hours").appendTo(datepicker.Calendar);
            var table = $("<table></table>").addClass("table-condensed").append("<thead></thead><tbody></tbody>")
                        .appendTo(container);
            $("thead", table).append("<tr></tr>");
            $("thead > tr", table).append($("<th></th>").addClass("prev").css("visibility", "hidden").append($("<i></i>").addClass("icon-arrow-left")));
            $("thead > tr", table).append($('<th colspan="5"></th>').addClass("switch"));
            $("thead > tr", table).append($("<th></th>").addClass("next").css("visibility", "hidden").append($("<i></i>").addClass("icon-arrow-right")));

            $("tbody", table).append("<tr></tr>");
            $("tbody > tr", table).append('<td colspan="7"><span class="hour">00</span><span class="hour">01</span><span class="hour">02</span>'
                + '<span class="hour">03</span><span class="hour">04</span><span class="hour">05</span>'
                + '<span class="hour">06</span><span class="hour">07</span><span class="hour">08</span>'
                + '<span class="hour">09</span><span class="hour">10</span><span class="hour">11</span>'
                + '<span class="hour">12</span><span class="hour">13</span><span class="hour">14</span>'
                + '<span class="hour">15</span><span class="hour">16</span><span class="hour">17</span>'
                + '<span class="hour">18</span><span class="hour">19</span><span class="hour">20</span>'
                + '<span class="hour">21</span><span class="hour">22</span><span class="hour">23</span></td>');

            datepicker._BuildCalendarContentForHours();
        },
        _BuildCalendarContentForHours: function () {
            var datepicker = this, options = this.Options;
            var table = $(".frame-datepicker-hours > .table-condensed", datepicker.Calendar);
            datepicker._SetHeadText(table);

            $("tbody > tr > td span.active", table).removeClass("active");
            $("tbody > tr > td", table).children().each(function (i) {
                if ($(this).text() == datepicker.Now.Hour)
                    $(this).addClass("active");
            }).on("click", function () {
                datepicker.ActiveDate.Hour = $(this).text();
                $("label.for-hour", datepicker.Calendar.TimeContainer).text(datepicker.ActiveDate.Hour);
                $(".frame-datepicker-days", datepicker.Calendar).show();
                $(".frame-datepicker-hours", datepicker.Calendar).hide();
            });
        },
        _CreateCalendarForMinutes: function () {
            var datepicker = this, options = this.Options;
            var container = $("<div></div>").addClass("frame-datepicker-minutes").appendTo(datepicker.Calendar);
            var table = $("<table></table>").addClass("table-condensed").append("<thead></thead><tbody></tbody>")
                        .appendTo(container);
            $("thead", table).append("<tr></tr>");
            $("thead > tr", table).append($("<th></th>").addClass("prev").css("visibility", "visible").append($("<i></i>").addClass("icon-arrow-left")));
            $("thead > tr", table).append($('<th colspan="5"></th>').addClass("switch"));
            $("thead > tr", table).append($("<th></th>").addClass("next").css("visibility", "visible").append($("<i></i>").addClass("icon-arrow-right")));

            $("tbody", table).append("<tr></tr>");
            $("tbody > tr", table).append('<td colspan="7"><span class="minute">00</span><span class="minute">01</span>'
                + '<span class="minute">02</span><span class="minute">03</span><span class="minute">04</span>'
                + '<span class="minute">05</span><span class="minute">06</span><span class="minute">07</span>'
                + '<span class="minute">08</span><span class="minute">09</span><span class="minute">10</span>'
                + '<span class="minute">11</span><span class="minute">12</span><span class="minute">13</span>'
                + '<span class="minute">14</span><span class="minute">15</span><span class="minute">16</span>'
                + '<span class="minute">17</span><span class="minute">18</span><span class="minute">19</span>'
                + '<span class="minute">20</span><span class="minute">21</span><span class="minute">22</span>'
                + '<span class="minute">23</span><span class="minute">24</span><span class="minute">25</span>'
                + '<span class="minute">26</span><span class="minute">27</span><span class="minute">28</span>'
                + '<span class="minute">29</span><span class="minute">30</span><span class="minute">31</span>'
                + '<span class="minute">32</span><span class="minute">33</span><span class="minute">34</span>'
                + '<span class="minute">35</span><span class="minute">36</span><span class="minute">37</span>'
                + '<span class="minute">38</span><span class="minute">39</span><span class="minute">40</span>'
                + '<span class="minute">41</span><span class="minute">42</span><span class="minute">43</span>'
                + '<span class="minute">44</span><span class="minute">45</span><span class="minute">46</span>'
                + '<span class="minute">47</span><span class="minute">48</span><span class="minute">49</span>'
                + '<span class="minute">50</span><span class="minute">51</span><span class="minute">52</span>'
                + '<span class="minute">53</span><span class="minute">54</span><span class="minute">55</span>'
                + '<span class="minute">56</span><span class="minute">57</span><span class="minute">58</span>'
                + '<span class="minute">59</span></td>');

            datepicker._BuildCalendarContentForMinutes();
        },
        _BuildCalendarContentForMinutes: function () {
            var datepicker = this, options = this.Options;
            var table = $(".frame-datepicker-minutes > .table-condensed", datepicker.Calendar);
            datepicker._SetHeadText(table);

            $("tbody > tr > td span.active", table).removeClass("active");
            $("tbody > tr > td", table).children().each(function (i) {
                if ($(this).text() == datepicker.Now.Minute)
                    $(this).addClass("active");
            }).on("click", function () {
                datepicker.ActiveDate.Minute = $(this).text();
                $("label.for-minute", datepicker.Calendar.TimeContainer).text(datepicker.ActiveDate.Minute);
                $(".frame-datepicker-days", datepicker.Calendar).show();
                $(".frame-datepicker-minutes", datepicker.Calendar).hide();
            });
        },
        _OnCalendarResize: function () {
            ///<summary>
            /// 重新定位日历部分显示位置
            ///</summary>
            var datepicker = this, options = this.Options;

            var contentHeight = $(document).height();

            var left = $(datepicker.Element).parent().offset().left;
            var top = $(datepicker.Element).parent().offset().top + $(datepicker.Element).parent().outerHeight(true) + 5;

            if (Number(top + datepicker.Calendar.height()) > contentHeight
            && contentHeight > Number(datepicker.Calendar.height() + 1)) {
                //若下拉框大小超过当前document下边框,且当前document上留白大于下拉内容高度,下拉内容向上展现
                top = $(datepicker.Element).parent().offset().top - 1 - datepicker.Calendar.outerHeight(true);
            }

            datepicker.Calendar.css("left", left).css("top", top);
        },
        _SetShowType: function (type) {
            var datepicker = this, options = this.Options;
            switch (type) {
                case "ymd":
                    $(".frame-datepicker-days", datepicker.Calendar).show();
                    $(".frame-datepicker-months", datepicker.Calendar).hide();
                    $(".frame-datepicker-years", datepicker.Calendar).hide();
                    $(".frame-datepicker-hours", datepicker.Calendar).hide();
                    $(".frame-datepicker-minutes", datepicker.Calendar).hide();
                    datepicker._BuildCalendarContentForDays();
                    break;
                case "ym":
                    $(".frame-datepicker-months", datepicker.Calendar).show();
                    $(".frame-datepicker-days", datepicker.Calendar).hide();
                    $(".frame-datepicker-years", datepicker.Calendar).hide();
                    $(".frame-datepicker-hours", datepicker.Calendar).hide();
                    $(".frame-datepicker-minutes", datepicker.Calendar).hide();
                    datepicker._BuildCalendarContentForMonths();
                    break;
            }
        },
        _SetCanShowTime: function (can) {
            var datepicker = this, options = this.Options;
            if (options.ShowType == "ymd") {
                if (can) {
                    $(".frame-datepicker-panel-foot", datepicker.Calendar).addClass("on-time");
                    datepicker.Calendar.TimeContainer.show();
                } else {
                    $(".frame-datepicker-panel-foot", datepicker.Calendar).removeClass("on-time");
                    datepicker.Calendar.TimeContainer.hide();
                }
            }
        },
        _IsDateTime: function (date) {
            ///<summary>
            /// 返回一个值，该值标识日期格式是否符合短日期格式，即yyyy-MM-dd
            ///</summary>
            ///<param name="date" type="string">日期值</param>
            var datepicker = this, options = this.Options;
            var r = date.match(/^(\d{4})(-|\/)(\d{2})\2(\d{2})$/);
            if (r == null) return false;
            var d = new Date(r[1], r[3] - 1, r[4]);
            if (d == "NaN") return false;
            return (d.getFullYear() == r[1] && (d.getMonth() + 1) == r[3] && d.getDate() == r[4]);
        },
        _IsLongDateTime: function (date) {
            ///<summary>
            /// 返回一个值，该值标识日期格式是否符合长日期格式，即yyyy-MM-dd hh:mm
            ///</summary>
            ///<param name="date" type="string">日期值</param>
            var datepicker = this, options = this.Options;
            var reg = /^(\d{4})(-|\/)(\d{2})\2(\d{2}) (\d{2}):(\d{2})$/;
            var r = date.match(reg);
            if (r == null) return false;
            var d = new Date(r[1], r[3] - 1, r[4], r[5], r[6]);
            if (d == "NaN") return false;
            return (d.getFullYear() == r[1] && (d.getMonth() + 1) == r[3] && d.getDate() == r[4] && d.getHours() == r[5] && d.getMinutes() == r[6]);
        },
        _GetDefaultDate: function () {
            ///<summary>
            /// 获取当前日期值，日期值按照指定格式转换
            ///</summary>
            var datepicker = this, options = this.Options;

            var format = options.DateFormat;
            if (options.ShowType == "ymd" && !options.CanShowTime) {
                format = options.DateFormat.substring(0, 10);
            }
            else if (options.ShowType == "ym") {
                format = options.DateFormat.substring(0, 7);
            }
            options.DateFormat = format;
            var value = datepicker.Now.ToDateTime().Format(format);

            if (datepicker.HasBind("Extractor"))
                value = datepicker.Trigger("Extractor", [value]);

            return value;
        },
        _SetDate: function () {
            ///<summary>
            /// 设置文本框值
            ///</summary>
            var datepicker = this, options = this.Options;
            var value = datepicker._GetDefaultDate();

            datepicker._SetValue(value);
        },
        _SetValue: function (value) {
            ///<summary>
            /// 设置文本框显示的日期值
            ///</summary>
            ///<param name="value" type="string">日期值</param>
            var datepicker = this, options = this.Options;

            if (value == "")
                return;

            var val = datepicker._Resolve(value);

            // 若传入参数的日期值经过验证后失败，则val为null，使用当前日期默认值进行转换赋值
            value = ((val == null) ? datepicker._GetDefaultDate() : new Date(val).Format(options.DateFormat));

            // 这里进行进一步解析处理的条件必须value的值不是通过调用_GetDefaultDate()方法得到，
            // 因为_GetDefaultDate()方法中已调用该事件
            if (datepicker.HasBind("Extractor") && val != null)
                value = datepicker.Trigger("Extractor", [value]);

            $(datepicker.Element).val(value);
            $(datepicker.Element).trigger("change");
        },
        _GetValue: function () {
            var datebox = this, options = this.Options;
            return $(datebox.Element).val();
        },
        _Resolve: function (dateString) {
            var datepicker = this, options = this.Options;
            var val = null;

            // 日期格式验证
            //-----------------------------------------------------------------
            // 若配置不显示时间，即表示日期格式为短日期格式
            if (datepicker.HasBind("Resolve"))
                dateString = datepicker.Trigger("Resolve", [dateString]);

            if (!options.CanShowTime && datepicker._IsDateTime(dateString))
                val = dateString.replace(/-/g, "/");
            // 若配置显示时间，即表示日期格式为长日期格式
            else if (options.CanShowTime && datepicker._IsLongDateTime(dateString))
                val = dateString.replace(/-/g, "/");


            if (val != null) {
                var date = new Date(val);
                datepicker.Now.Year = date.getFullYear().toString();
                datepicker.Now.Month = (date.getMonth() + 1) < 10 ? ("0" + (date.getMonth() + 1)) : (date.getMonth() + 1).toString();
                datepicker.Now.Day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate().toString();
                if (options.CanShowTime) {
                    datepicker.Now.Hour = date.getHours() < 10 ? "0" + date.getHours() : date.getHours().toString();
                    datepicker.Now.Minute = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes().toString();
                    $("label.for-hour", datepicker.Calendar.TimeContainer).text(datepicker.Now.Hour);
                    $("label.for-minute", datepicker.Calendar.TimeContainer).text(datepicker.Now.Minute);
                }
            }

            return val;
        },
        _SetIsShowDate: function (value) {
            var datepicker = this, options = this.Options;
            var date = datepicker._GetDefaultDate();
            if (value) {
                $(datepicker.Element).val(date);
            }
        },
        _SetDisabled: function (value) {
            var datepicker = this, options = this.Options;
            datepicker.Disabled = value;
            if (value) {
                $(datepicker.Element).attr("disabled", value);
            }
            else {
                $(datepicker.Element).removeAttr("disabled");
            }
        },
        _GetDisabled: function () {
            var datepicker = this, options = this.Options;
            return datepicker.Disabled;
        },
        _SetReadOnly: function (value) {
            ///<summary>
            /// 设置文本框是否为只读
            ///</summary>
            ///<param name="value" type="bool">设置一个值，该值标识文本框是否为只读。</param>
            var datepicker = this, options = this.Options;
            if (value != null) {
                if (value)
                    $(datepicker.Element).attr("readonly", "readonly");
                else
                    $(datepicker.Element).removeAttr("readonly");
            }
        },
        _GetReadOnly: function () {
            var datepicker = this, options = this.Options;
            return $(datepicker.Element).attr("readonly");
        },
        SetValue: function (value) {
            var datepicker = this, options = this.Options;
            datepicker._SetValue(value);
        },
        GetValue: function () {
            var datepicker = this, options = this.Options;
            return datepicker._GetValue();
        },
        Disable: function () {
            var datepicker = this, options = this.Options;
            datepicker.Set({ Disabled: true });
        },
        Enable: function () {
            var datepicker = this, options = this.Options;
            datepicker.Set({ Disabled: false });

        }
    });

})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.06.15
* 说明：DropDownButton 下拉按钮控件。
*/
(function ($) {
    $.fn.DropDownButton = function (options) {
        return $.Frame.Do.call(this, "DropDownButton", arguments);
    };

    $.Methods.DropDownButton = $.Methods.DropDownButton || {};

    $.Defaults.DropDownButton = {
        Text: "",
        Size: null,
        Mode: "primary",
        MenuItems: [],   //{Text,Divider}
        Disabled: false,
        OnItemClick: null
    };

    $.Frame.Controls.DropDownButton = function (element, options) {
        $.Frame.Controls.DropDownButton.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.DropDownButton.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "DropDownButton";
        },
        _IdPrev: function () {
            return "DropDownButtonFor";
        },
        _ExtendMethods: function () {
            return $.Methods.DropDownButton;
        },
        _PreRender: function () {
            var ddbutton = this;

            if (!$(ddbutton.Element).hasClass("frame-button-group")) {
                $(ddbutton.Element).addClass("frame-button-group");
            }
        },
        _Render: function () {
            var ddbutton = this, options = this.Options;

            ddbutton.Btn = $("<button></button>").addClass("frame-button").appendTo(ddbutton.Element);
            ddbutton.Menu = $("<ul></ul>").addClass("frame-dropdown-menu").addClass("frame-dropdown-" + options.Mode);
            $(ddbutton.Element).append(ddbutton.Menu);

            ddbutton.Btn = ddbutton.Btn.Button($.extend(options, { Icon: "icon-caret-down", IconSite: "right", OnClick: function () {
                if ($(ddbutton.Menu).children().length > 0) {
                    ddbutton.Menu.width($(ddbutton.Element).width());
                    $(ddbutton.Element).toggleClass("open");
                }
            }
            }));

            ddbutton.Set(options);
        },
        _SetMenuItems: function (items) {
            var ddbutton = this, options = this.Options;
            if (items && items.length > 0) {
                $(items).each(function (idx) {
                    if (this.Divider) {
                        $("<li></li>").addClass("divider").appendTo(ddbutton.Menu);
                    }
                    else if (this.Text) {
                        var abtn = $("<a></a>").attr("href", "#").attr("idx", idx).text(this.Text).on("click", function () {
                            if (ddbutton.HasBind("ItemClick"))
                                ddbutton.Trigger("ItemClick", [$(this).text(), $(this).attr("idx")]);
                            $(ddbutton.Element).toggleClass("open");
                            return false;
                        });
                        var li = $("<li></li>").appendTo(ddbutton.Menu);
                        li.append(abtn);
                    }
                });
            }
        },
        Disable: function () {
            var ddbutton = this, options = this.Options;
            ddbutton.Btn.Disable();
        },
        Enable: function () {
            var ddbutton = this, options = this.Options;
            ddbutton.Btn.Enable();
        },
        _GetDisabled: function () {
            var ddbutton = this, options = this.Options;
            return ddbutton.Btn.Disabled;
        }
    });

})(jQuery);

/**
* jQuery FrameUI 1.2.2
* 
* 作者：zlx
* 日期：2014.08.17
* 说明：拖曳插件。 
*
*/
(function ($) {
    $.fn.Draggable = function (options) {
        options = $.extend({
            IdAttr: "FrameDraggableId",   // 表示该拖曳对象对应生成的DraggableId,此ID值附加在标签的frame属性上
            HasElement: false,            // 标识该拖曳对象的Element对象是否有使用
            PropertyToElement: "Target"   // 表示该拖曳对象对应的标签的jQuery对象名称
        }, options || {});

        return $.Frame.Do.call(this, "Draggable", arguments, options);
    };

    $.Defaults.Draggable = {
        Handler: null,      // 可进行拖动的句柄元素对象，这里可以是元素对象的ID或者元素jQuery对象
        Proxy: true,        // 标识拖动时是否使用代理元素
        ProxyX: null,       // 进行拖曳时，代理元素对象的起始X坐标点位置
        ProxyY: null,       // 进行拖曳时，代理元素对象的起始Y坐标点位置
        CanRevert: false,   // 标识元素拖动结束之后是否归位
        Animate: true,      // 标识元素拖曳时是否使用动画效果
        Receive: null,      // 接受显示拖动对象的容器元素对象，该属性与事件OnDragEnter,OnDragOver,OnDragLeave相关
        Disabled: false,    // 标识是否禁止拖动
        OnStartDrag: null,
        OnDrag: null,
        OnStopDrag: null,
        OnRevert: null,
        OnEndRevert: null,
        OnDragEnter: null,
        OnDragOver: null,
        OnDragLeave: null,
        OnDrop: null
    };

    $.Frame.Controls.Draggable = function (options) {
        $.Frame.Controls.Draggable.Parent.constructor.call(this, null, options);
    };

    $.Frame.Controls.Draggable.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Draggable";
        },
        _IdPrev: function () {
            return "DraggableIdFor";
        },
        _PreRender: function () {
            // 为标签目标对象创建FrameDraggableId属性并赋值其在组件管理器中的key值
            // 注意:这个属性用于指导创建Draggable对象后，返回结果对象时使用，该属性必须与Options的IdAttr值相匹配
            this.Options.Target.FrameDraggableId = this.Id;

            if (!$(this.Options.Target).hasClass("frame-draggable"))
                $(this.Options.Target).addClass("frame-draggable");

            // 初始化Draggable对象必须的变量
            $.extend(this, {
                Target: $(this.Options.Target), // 标签元素对应的jQuery对象
                Cursor: null,
                Handler: null,
                ReceiveEntered: null,
                CanRevert: false,
                Receive: null,
                Reverting: null,
                Position: null,
                Proxy: null,
                Disabled: false
            });
        },
        _Render: function () {
            var draggable = this, options = this.Options;
            draggable.Set(options);  // 调用以_Set前缀的函数方法，进行初始化
            draggable.Set("Cursor", "move"); // 这里初始化标识鼠标拖动元素时鼠标指针的样式，调用_SetCursor函数方法

            draggable.Handler.bind("mousedown.Draggable", function (e) {
                // 禁用Draggable对象，此时将无法进行拖动
                if (draggable.Disabled == true) return;
                // 标识鼠标右键，此时也无法进行拖动
                if (e.button == 2) return;

                // 开始进行拖动
                draggable._Start.call(draggable, e);
            }).bind("mousemove.Draggable", function (e) {
                if (draggable.Disabled == true) return;
                draggable.Handler.css("cursor", draggable.Cursor);
            });
        },
        _SetHandler: function (handler) {
            ///<summary>
            /// 设置拖动句柄对象
            ///</summary>
            ///<param name="handler" type="string">拖动句柄对象的标签ID或者jQuery对象.这里可以是</param>
            var draggable = this, options = this.Options;

            if (!handler) {
                // 当未设置句柄，则设置拖曳对象自身为句柄对象
                draggable.Handler = $(draggable.Target);
            }
            else {
                // 当参数为字符串，则表示设置标签拖曳对象元素中指定元素为句柄，否则为指定对应jQuery对象为句柄
                // 例：
                //    1、若Draggable对象目标元素结构为<div id="draggable"><div id="hand"></div></div>
                //    要指定id为hand的元素作为拖曳对象draggable的句柄，这里传入的handler参数为#hand
                //    2、若Draggable对象目标元素结构为<div id="draggable"><div id="hand"></div></div>
                //    要指定id为hand的元素作为拖曳对象draggable的句柄，这里传入的handler参数也可为jQuery对象:$("#hand")
                //    3、若Draggable对象目标元素结构为<div id="draggable"><div class="hand"></div></div>
                //    要指定class为hand的元素作为拖曳对象draggable的句柄，这里传入的handler参数为.hand
                draggable.Handler = (typeof handler == "string" ? $(handler, draggable.Target) : $(handler));
            }
        },
        _SetReceive: function (receive) {
            ///<summary>
            /// 设置接受目标元素的容器对象
            ///</summary>
            ///<param name="receive" type="string">接收拖曳对象的容器对象的标签ID或者jQuery对象</param>

            // 标识目标元素对象是否进入接收容器元素对象区域中
            this.ReceiveEntered = [];

            if (!receive) return;

            if (typeof receive == "string")
                this.Receive = $(receive);
            else
                this.Receive = receive;

        },
        _SetCursor: function (cursor) {
            this.Cursor = cursor;
            (this.Proxy || this.Handler).css("cursor", cursor);
        },
        _SetCanRevert: function (value) {
            this.CanRevert = value;
        },
        _GetCanRevert: function () {
            return this.CanRevert;
        },
        _SetDisabled: function (value) {
            this.Disabled = value;
        },
        _GetDisabled: function () {
            return this.Disabled;
        },
        _CreateProxy: function (proxy, e) {
            ///<summary>
            /// 创建代理
            ///</summary>
            ///<param name="proxy" type="selector">代理标签元素对应的jQuery对象</param>
            ///<param name="e" type="object">鼠标事件对象</param>
            if (!proxy) return;

            var draggable = this, options = this.Options;

            if (typeof proxy == "function") {
                // 当proxy为函数对象时，函数接收两个参数:当前Draggable对象和鼠标事件对象
                draggable.Proxy = proxy.call(draggable.Target, draggable, e);
            }
            else if (proxy == "Clone") {
                draggable.Proxy = draggable.Target.clone().css("position", "absolute");
                //draggable.Proxy.appendTo("body");
                draggable.Proxy.appendTo(draggable.Target.parent());
            }
            else {
                draggable.Proxy = $("<div></div>")
                    .css("overflow", "hidden")
                    .css("border-style", "solid")
                    .css("border-width", "1px")
                    .css("opacity", "0.5")
                    .css("filter", "alpha(opacity=50)")
                    .css("position", "absolute")
                    .css("top", "0")
                    .css("left", "0")
                    .css("z-index", "10001")
                    .css("background", "#f2f1f1")
                    .css("border-color", "#aaa");
                draggable.Proxy.width(draggable.Target.width()).height(draggable.Target.height());
                //draggable.Proxy.appendTo("body");
                draggable.Proxy.appendTo(draggable.Target.parent());
            }

            draggable.Proxy.css({
                left: options.ProxyX == null ? draggable.Position.Left : draggable.Position.StartX + options.ProxyX,
                top: options.ProxyY == null ? draggable.Position.Top : draggable.Position.StartY + options.ProxyY
            }).show();

            return true;
        },
        _RemoveProxy: function () {
            var draggable = this;
            if (draggable.Proxy) {
                draggable.Proxy.remove();
                draggable.Proxy = null;
            }
        },
        _Start: function (e) {
            ///<summary>
            /// 开始对目标元素进行拖动
            ///</summary>
            ///<param name="e" type="object">鼠标事件对象</param>
            var draggable = this, options = this.Options;
            if (draggable.Reverting == true) return;  // 标识正在恢复原位
            if (draggable.Disabled == true) return;

            //            draggable.Position = {
            //                Left: draggable.Target.offset().left,
            //                Top: draggable.Target.offset().top,
            //                StartX: e.pageX || e.screenX,
            //                StartY: e.pageY || e.clientY,
            //                DiffX: 0,
            //                DiffY: 0
            //            };

            draggable.Position = {
                Left: draggable.Target.position().left,
                Top: draggable.Target.position().top,
                StartX: e.pageX || e.screenX,
                StartY: e.pageY || e.clientY,
                DiffX: 0,
                DiffY: 0
            };

            if (draggable.Trigger("StartDrag", [draggable.Target, e]) == false) return false;

            //代理没有创建成功
            if (options.Proxy && !draggable._CreateProxy(options.Proxy, e)) return false;

            $(document).bind("selectstart.Draggable", function () { return false; });
            $(document).bind("mousemove.Draggable", function () {
                draggable._Drag.apply(draggable, arguments);
            });

            // 标识当前有拖曳对象正在进行拖动操作，这里Dragging是全局变量，同一时刻只允许一个元素标签进行拖动
            $.Frame.Draggable.Dragging = true;

            $(document).bind("mouseup.Draggable", function () {
                $.Frame.Draggable.Dragging = false; // 拖动操作完成，恢复Dragging的状态
                draggable._Stop.apply(draggable, arguments);
            });
        },
        _Drag: function (e) {
            ///<summary>
            /// 对目标元素进行拖动
            ///</summary>
            ///<param name="e" type="object">鼠标事件对象</param>
            var draggable = this, options = this.Options;
            if (!draggable.Position) return;

            // 当前鼠标移动所在的X、Y点位置
            var pageX = e.pageX || e.screenX;
            var pageY = e.pageY || e.screenY;

            // 元素对象在鼠标移动的两点间的位移差
            draggable.Position.DiffX = pageX - draggable.Position.StartX;
            draggable.Position.DiffY = pageY - draggable.Position.StartY;

            // 将目标元素对象置于接收容器元素对象中
            draggable._Receive(e);

            if (draggable.HasBind("Drag")) {
                if (draggable.Trigger("Drag", [draggable.Target, e]) != false) {
                    draggable._ApplyDrag();
                }
                else {
                    // 当触发Drag事件且返回值为false时，表示限制拖动，此时不发生拖动效果，且移除代理元素
                    draggable._RemoveProxy();
                }
            }
            else {
                draggable._ApplyDrag();
            }
        },
        _Stop: function (e) {
            var draggable = this, options = this.Options;
            $(document).unbind("mousemove.Draggable");
            $(document).unbind("mouseup.Draggable");
            $(document).unbind("selectstart.Draggable");
            if (draggable.Receive) {
                draggable.Receive.each(function (i, obj) {
                    if (draggable.ReceiveEntered[i]) {
                        draggable.Trigger("Drop", [obj, draggable.Proxy || draggable.Target, e]);
                    }
                });
            }
            if (draggable.Proxy) {
                if (options.CanRevert) {
                    if (draggable.HasBind("Revert")) {
                        if (draggable.Trigger("Revert", [e]) != false)
                            draggable._Revert(e);
                        else
                            draggable._RemoveProxy();
                    }
                    else {
                        draggable._Revert(e);
                    }
                }
                else {
                    draggable._ApplyDrag(draggable.Target);
                    draggable._RemoveProxy();
                }
            }

            draggable.Trigger("StopDrag", [draggable.Target, e]);
            draggable.Position = null;

        },
        _Revert: function (e) {
            ///<summary>
            /// 将目标元素对象进行归位
            ///</summary>
            ///<param name="e" type="object">鼠标事件对象</param>
            var draggable = this;
            draggable.Reverting = true;
            draggable.Proxy.animate({
                left: draggable.Position.Left,
                top: draggable.Position.Top
            }, function () {
                draggable.Reverting = false;
                draggable._RemoveProxy();
                draggable.Trigger("EndRevert", [e]);
                draggable.Position = null;
            });
        },
        _Receive: function (e) {
            ///<summary>
            /// 将目标元素或其代理元素拖动到指定容器元素当中
            ///</summary>
            ///<param name="e" type="object">鼠标事件对象</param>
            var draggable = this, options = this.Options;

            if (draggable.Receive) {
                draggable.Receive.each(function (i, obj) {
                    var receive = $(obj);
                    var xy = receive.offset();
                    if (e.pageX > xy.left && e.pageX < xy.left + receive.width() && e.pageY > xy.top && e.pageY < xy.top + receive.height()) {
                        if (!draggable.ReceiveEntered[i]) {
                            draggable.ReceiveEntered[i] = true;
                            draggable.Trigger("DragEnter", [obj, draggable.Proxy || draggable.Target, e]);
                        }
                        else {
                            draggable.Trigger("DragOver", [obj, draggable.Proxy || draggable.Target, e]);
                        }
                    }
                    else if (draggable.ReceiveEntered[i]) {
                        draggable.ReceiveEntered[i] = false;
                        draggable.Trigger("DragLeave", [obj, draggable.Proxy || draggable.Target, e]);
                    }
                });
            }
        },
        _ApplyDrag: function (applyResultBody) {
            ///<summary>
            /// 将目标元素对象拖动到指定位置，实现拖动
            ///</summary>
            ///<param name="applyResultBody" type="selector">进行拖动的目标元素的jQuery对象</param>
            var draggable = this, options = this.Options;
            var position = {}, changed = false;

            applyResultBody = applyResultBody || draggable.Proxy || draggable.Target;
            var noproxy = (applyResultBody == draggable.Target); // 两者对象相同表示没有使用代理对象

            // 计算水平位置移动的距离
            if (draggable.Position.DiffX) {
                if (noproxy || options.ProxyX == null) {
                    // 若不是使用代理对象,则移动的距离=当前位置+移动的间距
                    position.left = draggable.Position.Left + draggable.Position.DiffX;
                }
                else {
                    // 若是使用代理对象,则移动的距离=当前鼠标位置+代理配置的起始坐标位置+移动的间距
                    position.left = draggable.Position.StartX + options.ProxyX + draggable.Position.DiffX;
                }
                // 标识位置变化
                changed = true;
            }

            // 计算垂直位置移动的距离
            if (draggable.Position.DiffY) {
                if (noproxy || options.ProxyY == null)
                    position.top = draggable.Position.Top + draggable.Position.DiffY;
                else
                    position.top = draggable.Position.StartY + options.ProxyY + draggable.Position.DiffY;
                changed = true;
            }
            if (applyResultBody == draggable.Target && draggable.Proxy && options.Animate) {
                draggable.Reverting = true;
                applyResultBody.animate(position, function () {
                    draggable.Reverting = false;
                });
            }
            else {
                applyResultBody.css(position);
            }
        },
        Enable: function () {
            ///<summary>
            /// 启用Draggable，即可进行拖动
            ///</summary>
            this.Disabled = false;
            this.Options.Disabled = false;
        },
        Disable: function () {
            ///<summary>
            /// 禁用Draggable，即不可进行拖动
            ///</summary>
            this.Disabled = true;
            this.Options.Disabled = true;
        }
    });
})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.11
* 说明：FormBox 窗体控件。
*/
(function ($) {
    $.fn.FormBox = function (options) {
        return $.Frame.Do.call(this, "FormBox", arguments);
    };

    $.Methods.FormBox = $.Methods.FormBox || {};

    $.Defaults.FormBox = {
        HasHeader: true,
        Closable: false,
        Commands: [],       //{Icon,OnClick}
        Title: "标题",
        Icon: "icon-desktop",
        HasFooter: true,
        Buttons: [],
        Left: "auto",
        Top: "auto",
        CloseOnDestroy: false,
        CanDraggable: false,
        CanResizable: false,
        IsModal: false,
        Url: null,
        Params: {},
        Content: null,
        Width: "auto",
        Height: "auto",
        OnBeforeShow: null,
        OnShown: null,
        OnBeforeClose: null,
        OnClose: null
    };

    $.Frame.Controls.FormBox = function (element, options) {
        $.Frame.Controls.FormBox.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.FormBox.Extension($.Frame.Controls.WindowBase, {
        _GetType: function () {
            return "FormBox";
        },
        _IdPrev: function () {
            return "FormBoxFor";
        },
        _ExtendMethods: function () {
            return $.Methods.FormBox;
        },
        _PreRender: function () {
            var formbox = this;

            //            if (self.frameElement && self.frameElement.tagName == "IFRAME") {
            //                $(formbox.Element).appendTo(parent.window.document.body);
            //            }
            //            else {
            //                $(formbox.Element).appendTo("body");
            //            }
            $(formbox.Element).appendTo("body");
            if (!$(formbox.Element).hasClass("frame-formbox")) {
                $(formbox.Element).addClass("frame-formbox");
            }

            if ($(formbox.Element).children().length > 0) {
                this.Options.Content = $(formbox.Element).children().first().detach();
                $(formbox.Element).empty();
            }

            formbox.MinHeight = 65;
        },
        _Render: function () {
            var formbox = this, options = this.Options;

            formbox.Container = $("<div></div>").addClass("frame-formbox-inner").appendTo(formbox.Element);
            formbox.Header = $("<div></div>").addClass("frame-formbox-header").appendTo(formbox.Container);
            formbox.Content = $("<div></div>").addClass("frame-formbox-content").appendTo(formbox.Container);
            formbox.Footer = $("<div></div>").addClass("frame-formbox-footer").appendTo(formbox.Container);

            formbox.Set(options);

            formbox._SetCenter();
            $(formbox.Element).hide();
        },
        _SaveStatus: function () {
            ///<summary>
            /// 保存记录当前面板位置及大小.用于当面板调用还原或显示方法时
            ///</summary>
            var formbox = this;
            formbox._Width = $(formbox.Element).width();
            formbox._Height = $(formbox.Element).height();
            var top = $(formbox.Element).offset().top;
            var left = $(formbox.Element).offset().left;
            if (!isNaN(parseInt($(formbox.Element).css("top"))))
                top = parseInt($(formbox.Element).css("top"));
            if (!isNaN(parseInt($(formbox.Element).css("left"))))
                left = parseInt($(formbox.Element).css("left"));
            formbox._Top = top;
            formbox._Left = left;
        },
        _SetHasHeader: function (hasHeader) {
            var formbox = this, options = this.Options;
            if (!hasHeader) {
                formbox.Header.remove();
            }
        },
        _SetClosable: function (closable) {
            var formbox = this, options = this.Options;
            if (closable && options.HasHeader) {
                var button = $("<button></button>").addClass("close").text("×").on("click", function () {
                    if (options.CloseOnDestroy)
                        formbox.Destroy();
                    else
                        formbox.Close();
                });
                button.appendTo(formbox.Header);
            }
        },
        _SetCommands: function (commands) {
            var formbox = this, options = this.Options;
            if (commands && commands.length > 0 && options.HasHeader) {
                var toolbar = $("<div></div>").addClass("frame-formbox-toolbar").appendTo(formbox.Header);
                $(commands).each(function () {
                    var command = this;
                    var btn = $("<a></a>").append($("<i></i>").addClass(command.Icon)).appendTo(toolbar);
                    if (command.OnClick) {
                        btn.on("click", function () {
                            command.OnClick.call(this, formbox);
                        });
                    }
                });
            }
        },
        _SetTitle: function (title) {
            var formbox = this, options = this.Options;
            if (title && options.HasHeader) {
                $("<h4></h4>").addClass("frame-formbox-title").addClass("blue").addClass("bigger")
                              .append($("<span></span>").text(title)).appendTo(formbox.Header);
            }
        },
        _SetIcon: function (icon) {
            var formbox = this, options = this.Options;
            if (icon && options.HasHeader) {
                $("h4", formbox.Header).prepend($("<i></i>").addClass(icon));
            }
        },
        _SetHasFooter: function (hasFooter) {
            var formbox = this, options = this.Options;
            if (!hasFooter) {
                formbox.Footer.remove();
            }
        },
        _SetButtons: function (buttons) {
            var formbox = this, options = this.Options;
            if (buttons && buttons.length > 0 && options.HasFooter) {
                $(buttons).each(function () {
                    var button = $("<button></button>").Button(this);
                    button.Context = formbox;
                    var orientation = this.Orientation == "right" ? "pull-right" : "pull-left";
                    formbox.Footer.append($(button.Element).addClass(orientation));
                });
            }
        },
        _SetWidth: function (width) {
            var formbox = this, options = this.Options;

            var newWidth = $(formbox.Element).parent().width() + 2;
            if (width != "auto") {
                newWidth = width;
                $(formbox.Element).width(newWidth);
            } else {
                $(formbox.Element).width(newWidth);
            }

            formbox.Content.width(newWidth - 12);
        },
        _SetHeight: function (height) {
            var formbox = this, options = this.Options;

            var newHeight = $(formbox.Element).parent().height() + 2;
            var headerOfHeight = (options.HasHeader && formbox.Header) ? formbox.Header.outerHeight(true) : 0;
            var footerOfHeight = (options.HasFooter && formbox.Footer) ? formbox.Footer.outerHeight(true) : 0;

            if (height != "auto") {
                newHeight = height - headerOfHeight - footerOfHeight + 2;
                if (newHeight < formbox.MinHeight)
                    newHeight = formbox.MinHeight + headerOfHeight + footerOfHeight;
                $(formbox.Element).height(newHeight);
                formbox.Content.height(newHeight - headerOfHeight - footerOfHeight - 13);
            } else {
                newHeight = formbox.Content.outerHeight(true);
                formbox.Content.height(newHeight - 10);
                $(formbox.Element).height(newHeight + headerOfHeight + footerOfHeight + 2);
            }
        },
        _SetLeft: function (left) {
            var formbox = this, options = this.Options;
            if (left != "auto") {
                $(formbox.Element).css("left", left);
            }
        },
        _SetTop: function (top) {
            var formbox = this, options = this.Options;
            if (top != "auto") {
                $(formbox.Element).css("top", top);
            }
        },
        _SetCanDraggable: function (can) {
            var formbox = this, options = this.Options;
            if (can && options.HasHeader) {
                formbox.Header.mousedown(function () {
                    $.Frame.WindowBase.SetFront(formbox);
                });
                formbox._ApplyDraggable();
            }
        },
        _ApplyDraggable: function () {
            ///<summary>
            /// 应用拖曳功能，使面板可进行拖动
            ///</summary>
            var formbox = this;
            if ($.fn.Draggable) {
                formbox.Draggable = $(formbox.Element).Draggable({
                    Handler: ".frame-formbox-title",
                    Animate: false,
                    OnStartDrag: function (e) {
                        $.Frame.WindowBase.SetFront(formbox);
                    },
                    OnStopDrag: function (e) {
                        formbox._SaveStatus();
                    }
                });
            }
        },
        _SetCanResizable: function (can) {
            var formbox = this, options = this.Options;
            if (can) {
                formbox._ApplyResizable();
            }
        },
        _ApplyResizable: function () {
            ///<summary>
            /// 应用调整尺寸功能，使面板可进行大小调整
            ///</summary>
            var formbox = this, options = this.Options;
            if ($.fn.Resizable) {
                formbox.Resizable = $(formbox.Element).Resizable({ Scope: 5,
                    OnStartResize: function (current, e) {
                        formbox._Deceive = formbox._Deceive || formbox._CreateDeceive();
                        formbox._Deceive.width($(formbox.Content).width() - 10).height($(formbox.Content).height() - 10);
                        $(formbox.Content).append(formbox._Deceive);
                    },
                    OnStopResize: function (current, e) {
                        var top = 0;
                        var left = 0;
                        var p = $(formbox.Element);
                        if (!options.HasHeader)
                            p.removeClass("frame-resizable");
                        if (!isNaN(parseInt(p.css("top"))))
                            top = parseInt(p.css("top"));
                        if (!isNaN(parseInt(p.css("left"))))
                            left = parseInt(p.css("left"));
                        if (current.DiffLeft) {
                            formbox.Set({ Left: left + current.DiffLeft });
                        }
                        if (current.DiffTop) {
                            formbox.Set({ Top: top + current.DiffTop });
                        }
                        if (current.NewWidth) {
                            formbox.Set({ Width: current.NewWidth });
                        }
                        if (current.NewHeight) {
                            formbox.Set({ Height: current.NewHeight });
                        }
                        formbox._SaveStatus();
                        formbox._RemoveDeceive();
                        return false;
                    }
                });
            }
        },
        _CreateDeceive: function () {
            ///<summary>
            /// 创建蒙层
            ///</summary>            
            return $("<div></div>").css("display", "block")
                                    .css("overflow", "hidden")
                                    .css("border", "0")
                                    .css("opacity", "0")
                                    .css("position", "absolute")
                                    .css("top", "0")
                                    .css("left", "0")
                                    .css("z-index", "10001");
        },
        _RemoveDeceive: function () {
            var formbox = this, options = this.Options;
            if (formbox._Deceive) {
                formbox._Deceive.remove();
                formbox._Deceive = null;
            }
        },
        _SetContent: function (content) {
            var formbox = this, options = this.Options;
            if (content && !options.Url) {
                formbox.Content.append($(content).show());
            }
        },
        _SetUrl: function (url) {
            var formbox = this, options = this.Options;
            if (url) {
                if (options.Params) {
                    url += url.indexOf('?') == -1 ? "?" : "&";
                    for (var name in options.Params) {
                        url += (name + "=" + options.Params[name] + "&");
                    }
                    url += ("random=" + new Date().getTime());
                }
                var iframe = $("iframe", formbox.Content);
                if (iframe.length > 0) {
                    iframe.attr("src", url);
                } else {
                    var framename = "Frame" + new Date().getTime();
                    iframe = $('<iframe onLoad="$.iFrameResize(this);"></iframe>').attr("id", framename).attr("name", framename)
                                                                               .attr("width", "100%").attr("frameborder", "0")
                                                                               .attr("scrolling", "no").attr("src", url)
                                                                               .appendTo(formbox.Content);
                }
            }
        },
        Show: function (pos) {
            var formbox = this, options = this.Options;
            formbox._SetCenter();
            if (pos)
                formbox.Set({ Top: pos.Top, Left: pos.Left });

            if (formbox.HasBind("BeforeShow")) {
                formbox.Trigger("BeforeShow", []);
            }

            formbox.Mask();
            $.Frame.WindowBase.SetFront(formbox);
            $(formbox.Element).show();

            if (formbox.HasBind("Shown")) {
                formbox.Trigger("Shown", []);
            }
        },
        Close: function () {
            var formbox = this, options = this.Options;

            if (formbox.HasBind("BeforeClose")) {
                formbox.Trigger("BeforeClose", []);
            }

            formbox.UnMask();
            $(formbox.Element).hide();

            if (formbox.HasBind("Close")) {
                formbox.Trigger("Close");
            }
        },
        Destroy: function () {
            var formbox = this, options = this.Options;

            formbox.Close();
            $(formbox.Element).remove();
            // 从组件管理器中移除
            $.Frame.Remove(formbox);
        },
        Load: function (url, params) {
            var formbox = this, options = this.Options;
            if (params)
                options.Params = $.extend(options.Params, params);

            formbox.Set({ Url: url });
        },
        Reload: function (params) {
            var formbox = this, options = this.Options;
            var url = options.Url;
            formbox.Load(url, params);
        },
        _SetCenter: function () {
            var formbox = this, options = this.Options;

            var windowHeight = $(window).outerHeight(true) - window.screenTop;
            var windowWidth = $(window).width();
            //                        if (self.frameElement && self.frameElement.tagName == "IFRAME") {
            //                            windowHeight = $(parent.window).height();
            //                            windowWidth = $(parent.window).width();
            //                        }
            var panelHeight = $(formbox.Element).height();
            var panelWidth = $(formbox.Element).width();
            var left = 0, top = 0;

            var parentTarget = $(formbox.Element).parent();
            if (parentTarget.is("body")) {
                left = (windowWidth / 2) - (panelWidth / 2);
                top = (windowHeight / 2) + $(window).scrollTop() - (panelHeight / 2);
            }
            else {
                left = (parentTarget.width() / 2) - (panelWidth / 2);
                top = (parentTarget.height() / 2) - (panelHeight / 2);
            }

            formbox.Set({ Left: left, Top: top });
            formbox._SaveStatus();
        }
    });

    $.FormBox = function (options) {
        options = $.extend(options, { CloseOnDestroy: true });
        return $("<div></div>").FormBox(options);
    };

    $.FormBox.Show = function (options) {
        var form = $.FormBox(options);
        form.Show();
    };

})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.05.25
* 说明：Grid 表格控件。
*/
(function ($) {
    $.fn.Grid = function (options) {
        return $.Frame.Do.call(this, "Grid", arguments);
    };

    $.Methods.Grid = $.Methods.Grid || {};

    $.Defaults.Grid = {
        CanShowPager: true,
        CanShowCheckBox: false,
        CanShowRowNumber: false,
        CanShowLoading: false,
        Message: "正在加载数据,请稍候...",
        Page: 1,    // 当前页码
        PageSize: 10, // 每页显示的记录数
        FrozenColumns: [],     //column{Title,Field,Width,Align,Hidden,Formatter,Styler}
        Columns: [],           //column{Title,Field,Width,Align,Hidden,Formatter,Styler}
        DataSource: null,
        Method: "POST",
        DataType: "json",
        Params: {},
        Url: false,
        Striped: true,
        CanMultiple: false,
        PageButtons: [],
        Width: "auto",
        Height: "auto",
        OnSuccess: null,                       //成功获取服务器数据的事件
        OnSelectRow: null,                    //选择行事件
        OnUnSelectRow: null,                   //取消选择行事件
        OnBeforeCheckRow: null,                 //选择前事件，可以通过return false阻止操作(复选框)
        OnCheckRow: null,                    //选择事件(复选框) 
        OnBeforeCheckAllRow: null,              //选择前事件，可以通过return false阻止操作(复选框 全选/全不选)
        OnCheckAllRow: null,                    //选择事件(复选框 全选/全不选)
        OnBeforeShowData: null,                  //显示数据前事件，可以通过reutrn false阻止操作
        OnAfterShowData: null,                 //显示完数据事件
        OnError: null,                         //错误事件
        OnLoading: null,                        //加载时函数
        OnLoaded: null,                          //加载完函数
        OnComplete: null,                       //请求完成后回调函数 (请求成功或失败之后均调用)。
        OnExtractor: null
    };

    $.Frame.Controls.Grid = function (element, options) {
        $.Frame.Controls.Grid.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Grid.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Grid";
        },
        _IdPrev: function () {
            return "GridFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Grid;
        },
        _PreRender: function () {
            var grid = this, options = this.Options;

            // TODO：这里为了防止页面标签元素重复设置样式
            if (!$(grid.Element).hasClass("frame-grid")) {
                $(grid.Element).addClass("frame-grid");
            }

            grid.DataSource = [];
            grid.Selections = [];
        },
        _Render: function () {
            var grid = this, options = this.Options;

            grid.ViewContainer = $("<div></div>").addClass("frame-grid-view").appendTo(grid.Element);

            // 冻结部分
            grid._CreateFreezedContainer();

            // 常规部分
            grid._CreateGroovyContainer();

            grid.Set(options);
            grid.Resize();
        },
        _CreateFreezedContainer: function () {
            var grid = this, options = this.Options;

            grid.FreezeViewContainer = $("<div></div>").addClass("frame-grid-freeze-view").appendTo(grid.ViewContainer);
            grid.FreezeHeadContainer = $("<div></div>").addClass("frame-grid-freeze-hdiv").appendTo(grid.FreezeViewContainer);
            grid.FreezeHeadBox = $("<div></div>").addClass("frame-grid-freeze-hbox").appendTo(grid.FreezeHeadContainer);
            grid.FreezeHeadTable = $("<table></table>").addClass("frame-grid-freeze-htable").attr("cellpadding", "0")
                                                       .attr("cellspacing", "0")
                                                       .attr("border", "0")
                                                       .append($("<thead></thead>")
                                                                .append($("<tr></tr>")
                                                                    .addClass("frame-grid-freeze-labels")))
                                                       .appendTo(grid.FreezeHeadBox);
            grid.FreezeContentContainer = $("<div></div>").addClass("frame-grid-freeze-bdiv").appendTo(grid.FreezeViewContainer);
            grid.FreezeContentBox = $("<div></div>").addClass("frame-grid-freeze-bbox").appendTo(grid.FreezeContentContainer);
            grid.FreezeTable = $("<table></table>").addClass("frame-grid-freeze-btable").attr("cellpadding", "0")
                                                       .attr("cellspacing", "0")
                                                       .attr("border", "0")
                                                       .append("<tbody></tbody>").appendTo(grid.FreezeContentBox);
        },
        _CreateGroovyContainer: function () {
            var grid = this, options = this.Options;

            grid.GroovyViewContainer = $("<div></div>").addClass("frame-grid-groovy-view").appendTo(grid.ViewContainer);
            grid.GroovyHeadContainer = $("<div></div>").addClass("frame-grid-groovy-hdiv").appendTo(grid.GroovyViewContainer);
            grid.GroovyHeadBox = $("<div></div>").addClass("frame-grid-groovy-hbox").appendTo(grid.GroovyHeadContainer);
            grid.GroovyHeadTable = $("<table></table>").addClass("frame-grid-groovy-htable").attr("cellpadding", "0")
                                                       .attr("cellspacing", "0")
                                                       .attr("border", "0")
                                                       .append($("<thead></thead>")
                                                                .append($("<tr></tr>")
                                                                    .addClass("frame-grid-groovy-labels")))
                                                       .appendTo(grid.GroovyHeadBox);
            grid.GroovyContentContainer = $("<div></div>").addClass("frame-grid-groovy-bdiv").appendTo(grid.GroovyViewContainer);
            grid.GroovyContentBox = $("<div></div>").addClass("frame-grid-groovy-bbox").appendTo(grid.GroovyContentContainer);
            grid.GroovyTable = $("<table></table>").addClass("frame-grid-groovy-btable").attr("cellpadding", "0")
                                                       .attr("cellspacing", "0")
                                                       .attr("border", "0")
                                                       .append("<tbody></tbody>").appendTo(grid.GroovyContentBox);
        },
        _SetCanShowPager: function (showPager) {
            ///<summary>
            /// 是否使用分页
            ///</summary>
            ///<param name="showPager" type="bool">设置一个值，该值标识是否使用分页控件。</param>
            var grid = this, options = this.Options;

            if (showPager) {
                grid.Pagination = $("<div></div>").addClass("frame-pagination").appendTo(grid.Element);
                $.extend(options.Params, { page: options.Page, pagesize: options.PageSize });
                grid.Pagination = $(grid.Pagination).Pagination({ PageSize: options.PageSize, Buttons: options.PageButtons,
                    OnSelectPage: function (page, size) {
                        // page:当前页码,size:每页行数
                        options.Page = page;
                        $.extend(options.Params, { page: page, pagesize: size });
                        grid.Load({});
                    }
                });
            }
        },
        _SetCanShowLoading: function (loading) {
            var grid = this, options = this.Options;
            if (loading) {
                var overlay = $("<div></div>").addClass("frame-grid-overlay");
                var l = $("<div></div>").addClass("loading").text(options.Message);
                grid.ViewContainer.prepend(l).prepend(overlay);
            }
        },
        _ShowLoading: function () {
            var grid = this, options = this.Options;
            $(".frame-grid-overlay", grid.ViewContainer).show();
            $(".loading", grid.ViewContainer).show();
        },
        _CloseLoading: function () {
            var grid = this, options = this.Options;
            $(".frame-grid-overlay", grid.ViewContainer).hide();
            $(".loading", grid.ViewContainer).hide();
        },
        _SetUrl: function (url) {
            /// <summary>
            /// 请求远程数据并加载到表格
            ///</summary>
            ///<param name="url" type="string">请求的URL</param>
            var grid = this, options = this.Options;
            if (url) {
                options.Url = url;
                grid.Load(options.Params);
            }
        },
        _CreateHeaderTag: function () {
            ///<summary>
            /// 创建一个数据表头列
            ///</summary>
            return $("<th></th>").addClass("frame-grid-table-th-column").addClass("frame-grid-table-th-ltr");
        },
        _CreateHeaderColumnTag: function (text) {
            return $("<div></div>").addClass("frame-grid-sortable").text(text);
        },
        _CompareColumnWidth: function (original, compare) {
            /// <summary>
            /// 比较两个宽度值.
            ///</summary>
            ///<param name="original" type="number">原宽度</param>
            ///<param name="compare" type="number">要进行比较的宽度</param>
            if (original && original > compare)
                return original;
            else
                return compare;
        },
        _CreateCheckBoxColumnOnHead: function () {
            ///<summary>
            /// 创建冻结数据表头的复选框列
            ///</summary>
            var grid = this, options = this.Options;

            if (options.CanShowCheckBox) {
                var chk_header_column = grid._CreateHeaderTag();
                var chk_header_div = $("<div></div>").addClass("frame-grid-sortable").addClass("frame-grid-sortable-chk");
                chk_header_column.append(chk_header_div);
                $(".frame-grid-freeze-labels", grid.FreezeHeadTable).append(chk_header_column);

                if (options.CanMultiple) {
                    var chk = $("<input type='checkbox' />").attr("id", grid.Id + "_chk_all").appendTo(chk_header_div);
                    chk.CheckBox({
                        OnChange: function (checked) {
                            if (checked) {
                                grid.SelectAll();
                            }
                            else {
                                grid.UnSelectAll();
                            }
                        }
                    });
                }
            }
        },
        _CreateRowNumberColumnOnHead: function () {
            ///<summary>
            /// 创建冻结数据表头的行号列
            ///</summary>
            var grid = this, options = this.Options;

            if (options.CanShowRowNumber) {
                var rownum_column = grid._CreateHeaderTag();
                var rownum_div = $("<div></div>").addClass("frame-grid-sortable").addClass("frame-grid-sortable-num");
                rownum_column.append(rownum_div);
                $(".frame-grid-freeze-labels", grid.FreezeHeadTable).append(rownum_column);
            }
        },
        _CreateColumnOnHead: function (column, trow) {
            ///<summary>
            /// 创建数据表头列
            ///</summary>
            ///<param name="trow" type="object">数据行对象。</param>
            var grid = this, options = this.Options;

            var th_column = grid._CreateHeaderTag();
            trow.append(th_column);
            var col = grid._CreateHeaderColumnTag(column.Title);
            th_column.append(col);
            if (options.FitColumns)
                column.Width = grid._CompareColumnWidth(column.Width, col.width());
            col.width(column.Width);
            if (column.Align)
                th_column.css("text-align", column.Align);
        },
        _SetColumns: function (columns) {
            ///<summary>
            /// 创建常规数据表头
            ///</summary>
            ///<param name="columns" type="array">常规表头的列对象集合。</param>
            var grid = this, options = this.Options;

            if (columns && columns.length > 0) {
                $(columns).each(function () {
                    if (!this.Hidden) {
                        grid._CreateColumnOnHead(this, $(".frame-grid-groovy-labels", grid.GroovyHeadTable));
                    }
                });
            }

            if (!options.CanShowCheckBox && !options.CanShowRowNumber && options.FrozenColumns.length == 0) {
                grid.GroovyHeadBox.addClass("frame-grid-groovy-hbox-ltr");
                grid.GroovyContentContainer.addClass("frame-grid-groovy-bdiv-ltr");
            }
        },
        _SetFrozenColumns: function (columns) {
            ///<summary>
            /// 创建冻结数据表头
            ///</summary>
            ///<param name="columns" type="array">冻结表头的列对象集合。</param>
            var grid = this, options = this.Options;

            grid._CreateCheckBoxColumnOnHead();
            grid._CreateRowNumberColumnOnHead();

            if (columns && columns.length > 0) {
                $(columns).each(function () {
                    if (!this.Hidden) {
                        grid._CreateColumnOnHead(this, $(".frame-grid-freeze-labels", grid.FreezeHeadTable));
                    }
                });
            }

            if (!options.CanShowCheckBox && !options.CanShowRowNumber && columns.length == 0) {
                grid.FreezeViewContainer.hide();
            }
        },
        _SetWidth: function (width) {
            var grid = this, options = this.Options;
            if (width != "auto") {
                $(grid.Element).width(width);
                grid.ViewContainer.width(width);
            } else {
                var parentWidth = $(grid.Element).parent().width();
                grid.ViewContainer.width(parentWidth);
            }
        },
        _SetHeight: function (height) {
            var grid = this, options = this.Options;
            if (height != "auto") {
                $(grid.Element).height(height);
                grid.ViewContainer.height(height);
            } else {
                var parentHeight = $(grid.Element).parent().height();
                var pageHeight = options.CanShowPager ? 55 : 0;
                $(grid.Element).height(parentHeight);
                grid.ViewContainer.height(parentHeight - pageHeight);
            }
        },
        _SetDataSource: function (data) {
            var grid = this, options = this.Options;

            if (data) {
                if (options.CanShowPager)
                    grid.Pagination.Refresh(data.Total, data.Total == 0 ? 0 : options.Page);

                if (data.Rows.length > 0) {
                    grid.DataSource = data.Rows;

                    $(data.Rows).each(function (idx) {
                        grid._CreateFreezeRow(idx, this);
                        grid._CreateGroovyRow(idx, this);
                    });

                    if (options.Striped) {
                        $("tr:odd", grid.FreezeTable).addClass("frame-grid-priority-secondary");
                        $("tr:odd", grid.GroovyTable).addClass("frame-grid-priority-secondary");
                    }

                    grid._BindingRowStateOnHover(grid.FreezeTable, grid.GroovyTable);
                    grid._BindingRowStateOnHover(grid.GroovyTable, grid.FreezeTable);
                }
            }
        },
        _BindingRowStateOnHover: function (bindingTable, toggleTable) {
            ///<summary>
            /// 对数据行元素绑定mouseover,mouseout,click事件,实现鼠标hover样式及行单击选中样式
            ///</summary>
            var grid = this, options = this.Options;
            $("tr", bindingTable).on("mouseover mouseout click", function (event) {
                var idx = $(this).attr("r_idx");
                var row_toggle = $("tr.frame-grid-row[r_idx=" + idx + "]", toggleTable);
                switch (event.type) {
                    case "mouseover": grid._RowOnMouseOver(idx, this, row_toggle); break;
                    case "mouseout": grid._RowOnMouseOut(idx, this, row_toggle); break;
                    case "click": grid._RowOnClick(idx, this, row_toggle); break;
                }

                return false;
            });
        },
        _RowOnMouseOver: function (idx, row_freeze, row_groovy) {
            var grid = this, options = this.Options;
            $(row_freeze).addClass("frame-grid-state-hover");
            $(row_groovy).addClass("frame-grid-state-hover");
        },
        _RowOnMouseOut: function (idx, row_freeze, row_groovy) {
            var grid = this, options = this.Options;
            $(row_freeze).removeClass("frame-grid-state-hover");
            $(row_groovy).removeClass("frame-grid-state-hover");
        },
        _RowOnClick: function (idx, row_freeze, row_groovy) {
            var grid = this, options = this.Options;
            if (!options.CanMultiple) {
                // 单选模式
                grid._CancelOtherwiseRow(idx);
            }
            $(row_freeze).toggleClass("frame-grid-state-highlight");
            $(row_groovy).toggleClass("frame-grid-state-highlight");
            grid._ToggleRecord(idx);

            /* TODO: 这里作用除了切换复选框勾选状态,当绑定了BeforeCheckRow事件且通过return false阻止操作.
            行元素的高亮将会重新被取消 */
            if (!grid._ToggleCheckBox(idx)) {
                $(row_freeze).toggleClass("frame-grid-state-highlight");
                $(row_groovy).toggleClass("frame-grid-state-highlight");
                grid._ToggleRecord(idx);
            }
        },
        _ToggleCheckBox: function (idx) {
            ///<summary>
            /// 复选框勾选状态切换
            ///</summary>
            ///<param name="idx" type="number">指定行的复选框索引。</param>
            var grid = this, options = this.Options;

            if (options.CanShowCheckBox) {
                var chkState = $("#" + grid.Id + "_row_chk_" + idx).Frame("Options", "Checked");
                if (chkState)
                    $("#" + grid.Id + "_row_chk_" + idx).Frame().Checked(false);
                else {
                    var hook = true;
                    if (grid.HasBind("BeforeCheckRow"))
                        hook = grid.Trigger("BeforeCheckRow", [idx]); // idx为行索引
                    if (hook) {
                        $("#" + grid.Id + "_row_chk_" + idx).Frame().Checked(true);
                    }
                    return hook;
                }
            }

            return true;
        },
        _CancelOtherwiseRow: function (idx) {
            ///<summary>
            /// 若当前单击的行与被选中的行不相同时,取消被选中行的所有状态
            ///</summary>
            ///<param name="idx" type="number">指定行的复选框索引。</param>
            var grid = this, options = this.Options;
            var row_highlight_freeze = $("tr.frame-grid-state-highlight", grid.FreezeTable);
            var row_highlight_groovy = $("tr.frame-grid-state-highlight", grid.GroovyTable);

            /* 这里判断之前是否有被选择的行元素,若有,则用来与当前单击的行索引进行判断是否为同一行,
            如果是同一个行元素,则跳过;如果不是同一个行元素,则取消行元素高亮 */
            if (row_highlight_groovy.length > 0) {
                var select_idx = row_highlight_groovy.attr("r_idx");
                if (select_idx != idx) {
                    $(row_highlight_freeze).removeClass("frame-grid-state-highlight");
                    $(row_highlight_groovy).removeClass("frame-grid-state-highlight");
                    grid._ToggleCheckBox(select_idx);
                    grid._RemoveRecord(select_idx);
                }
            }

        },
        _CreateFreezeRow: function (idx, data) {
            var grid = this, options = this.Options;
            var row = $("<tr></tr>").attr("r_idx", idx).addClass("frame-grid-row").addClass("frame-grid-row-ltr");
            $("tbody", grid.FreezeTable).append(row);

            grid._CreateCheckBoxColumnOnRow(idx);
            grid._CreateRowNumberColumnOnRow(idx);

            if (options.FrozenColumns.length > 0) {
                $(options.FrozenColumns).each(function () {
                    grid._PackCellFetchRow(idx, grid.FreezeTable, this, data);
                });
            }
        },
        _CreateCheckBoxColumnOnRow: function (idx) {
            var grid = this, options = this.Options;
            if (options.CanShowCheckBox) {
                var chk_column = $("<td></td>");
                var chk_column_div = $("<div></div>").addClass("frame-grid-sortable").addClass("frame-grid-sortable-chk");
                chk_column.append(chk_column_div);
                $("tr.frame-grid-row[r_idx=" + idx + "]", grid.FreezeTable).append(chk_column);

                var chk = $("<input type='checkbox' />").attr("id", grid.Id + "_row_chk_" + idx).attr("idx", idx).appendTo(chk_column_div);
                chk.CheckBox({
                    OnChange: function (state) {
                        if (state && grid.HasBind("CheckRow")) {
                            var index = $(this.Element).attr("idx");
                            var row = grid._FindRecordData(index);
                            grid.Trigger("CheckRow", [index, row]); //index:行索引,row:数据行记录对象
                        }
                    }
                });
            }
        },
        _CreateRowNumberColumnOnRow: function (idx) {
            var grid = this, options = this.Options;
            if (options.CanShowRowNumber) {
                var rownum_column = $("<td></td>");
                var rownum_div = $("<div></div>").addClass("frame-grid-sortable").addClass("frame-grid-sortable-num").text(idx + 1);
                rownum_column.append(rownum_div);
                $("tr.frame-grid-row[r_idx=" + idx + "]", grid.FreezeTable).append(rownum_column);
            }
        },
        _CreateGroovyRow: function (idx, data) {
            var grid = this, options = this.Options;
            var row = $("<tr></tr>").attr("r_idx", idx).addClass("frame-grid-row").addClass("frame-grid-row-ltr");
            $("tbody", grid.GroovyTable).append(row);

            if (options.Columns.length > 0) {
                $(options.Columns).each(function () {
                    grid._PackCellFetchRow(idx, grid.GroovyTable, this, data);
                });
            }
        },
        _PackCellFetchRow: function (idx, table, column, record) {
            ///<summary>
            /// 包装渲染每一行的单元格
            ///</summary>
            ///<param name="idx" type="number">指定行索引。</param>
            ///<param name="table" type="selector">表格jQuery对象。</param>
            ///<param name="column" type="object">列对象。</param>
            ///<param name="record" type="object">行记录数据对象</param>
            var grid = this, options = this.Options;
            var row = $("tr.frame-grid-row[r_idx=" + idx + "]", table);

            var value = record[column.Field];
            var cell = $("<td></td>");
            var column_div = $("<div></div>").addClass("frame-grid-sortable").text(value);
            row.append(cell);
            cell.append(column_div);

            if (column.Formatter) {
                var result = column.Formatter.call(column.Formatter, idx, record, value);
                value = result || value;
                column_div.text(value);
            }
            if (column.Styler) {
                var style = column.Styler.call(column.Styler, idx, record, value);
                if (style) {
                    var reg = /<(\w+)[^>]*>.*<\/\1>/; //匹配HTML标签字符串
                    if (reg.test(style)) {
                        column_div.empty();
                        column_div = column_div.append($(style));
                    }
                    else {
                        column_div.attr("style", style);
                    }
                }
            }

            column_div.width(column.Width);
            if (column.Align)
                column_div.css("text-align", column.Align);

        },
        Resize: function () {
            var grid = this, options = this.Options;

            // 计算常规表格部分可得宽度
            var freezeWidth = grid.FreezeViewContainer.outerWidth(true);    // 这里获取冻结表格部分的宽度
            var groovyWidth = grid.ViewContainer.width() - freezeWidth - 1; // 用表格总宽度-冻结表格部分的宽度,得到常规表格的宽度

            grid.GroovyHeadBox.width(groovyWidth);         // 设置常规表格的表头容器宽度
            grid.GroovyContentBox.width(groovyWidth);      // 设置常规表格表体容器宽度
            grid.GroovyContentContainer.width(groovyWidth); // 设置常规表格表体主容器宽度

            // 计算并设置表格表体实际宽度
            var headHeight = grid.FreezeHeadContainer.outerHeight(true);
            if (!options.CanShowCheckBox && !options.CanShowRowNumber && options.FrozenColumns.length == 0) {
                headHeight = grid.GroovyHeadContainer.outerHeight(true);
            }
            var bodyHeight = grid.ViewContainer.height() - headHeight;

            // 设置冻结表格部分的表体高度
            grid.FreezeContentContainer.height(bodyHeight);
            grid.FreezeContentBox.height(bodyHeight);
            // 设置常规表格部分的表体高度
            grid.GroovyContentContainer.height(bodyHeight);
            grid.GroovyContentBox.height(bodyHeight);

            grid._SetScroll(groovyWidth, bodyHeight);
            grid._ApplyScroll();
        },
        _SetScroll: function (width, height) {
            var grid = this, options = this.Options;

            var twidth = grid.GroovyTable.width();
            var theight = grid.GroovyTable.height();
            if (twidth > width) {
                // 出现水平滚动条
                grid.FreezeContentBox.height(height - 17);
                grid.GroovyContentBox.height(height - 17);
            }

            if (theight > height) {
                // 出现垂直滚动条
                if (twidth > width) {
                    grid.GroovyHeadBox.width(width - 17);
                }
                grid.GroovyContentBox.width(twidth);
            }
        },
        _ApplyScroll: function () {
            var grid = this, options = this.Options;
            grid.GroovyContentContainer.scroll(function () {
                grid.FreezeContentBox.scrollTop(grid.GroovyContentContainer.scrollTop());
                grid.GroovyHeadBox.scrollLeft(grid.GroovyContentContainer.scrollLeft());
            });
        },
        LoadData: function (data) {
            ///<summary>
            /// 加载本地数据,旧记录将会被删除
            ///</summary>
            ///<param name="data" type="object">本地数据对象,对象格式:{Total,Rows}。</param>
            var grid = this, options = this.Options;
            grid.Loading = true;
            if (grid.HasBind("Extractor")) {
                data = grid.Trigger("Extractor", [data]);
            }

            grid.Clear();

            if (grid.HasBind("BeforeShowData")) {
                data = grid.Trigger("BeforeShowData", [data]);
                if (!data)
                    return;
            }
            grid.Set({ DataSource: data });

            if (grid.HasBind("AfterShowData")) {
                grid.Trigger("AfterShowData", []);
            }
            grid.Resize();
        },
        Clear: function () {
            ///<summary>
            /// 清空数据表格的数据,若有启用分页,则同时设置分页初始状态
            ///</summary>
            var grid = this, options = this.Options;

            grid.DataSource = [];
            grid.Selections = [];
            $("tbody", grid.FreezeTable).empty();
            $("tbody", grid.GroovyTable).empty();
            if (options.CanShowPager)
                grid.Pagination.Refresh(0, 0);
        },
        Load: function (param) {
            ///<summary>
            /// 从服务器远程请求数据加载
            ///</summary>
            ///<param name="param" type="object">请求参数,两种参数格式:1.Url,2.Url、{"":""}</param>
            var grid = this, options = this.Options;
            if (arguments.length > 1) {
                options.Url = arguments[0];
                param = arguments[1];
            }
            if (arguments.length == 1 && typeof arguments[0] == "string") {
                options.Url = arguments[0];
                param = {};
            }
            $.extend(param, options.Params);
            if (grid.Loading || !options.Url)
                return;

            $.ajax({
                dataType: options.DataType, type: options.Method, url: options.Url, data: param,
                beforeSend: function () {
                    if (grid.HasBind("Loading")) {
                        grid.Trigger("Loading");
                    }
                    else {
                        if (options.CanShowLoading)
                            grid._ShowLoading();
                    }
                },
                success: function (data) {
                    if (grid.HasBind("Success")) {
                        data = grid.Trigger("Success", [data]);
                    }
                    if (data != null) {
                        grid.LoadData(data);
                    }
                },
                error: function (xhr, info, thrown) {
                    if (grid.HasBind("Error")) {
                        grid.Trigger("Error", [xhr, info, thrown]);
                    }
                    if (info == "error") {
                        $.Messager.Alert("错误提示", thrown, "error");
                    }
                },
                complete: function (xhr, ts) {
                    grid.Loading = false;
                    grid.Trigger("Complete", [xhr, ts]);
                    if (ts == "success") {
                        if (grid.HasBind("Loaded")) {
                            grid.Trigger("Loaded", [xhr, ts]);
                        }
                    }
                    if (options.CanShowLoading)
                        grid._CloseLoading();
                }
            });

        },
        SelectRow: function (index) {
            /// <summary>
            /// 选中一行数据
            ///</summary>
            ///<param name="index" type="number">行索引,索引从0起始</param>
            var grid = this, options = this.Options;

            var idx = grid._InSelected(index);

            if (idx == -1) {
                // 还未被选中
                var row_freeze = $("tr.frame-grid-row[r_idx=" + index + "]", grid.FreezeTable);
                var row_groovy = $("tr.frame-grid-row[r_idx=" + index + "]", grid.GroovyTable);
                grid._RowOnClick(index, row_freeze, row_groovy);
            }
        },
        UnSelectRow: function (index) {
            var grid = this, options = this.Options;

            var idx = grid._InSelected(index);

            if (idx > -1) {
                var row_freeze = $("tr.frame-grid-row[r_idx=" + index + "]", grid.FreezeTable);
                var row_groovy = $("tr.frame-grid-row[r_idx=" + index + "]", grid.GroovyTable);
                grid._RowOnClick(index, row_freeze, row_groovy);
            }
        },
        GetSelections: function () {
            /// <summary>
            /// 获取选中的数据记录
            ///</summary>
            var grid = this, options = this.Options;

            if (grid.Selections.length > 0) {
                return grid.Selections;
            }
            else
                return false;
        },
        _FindRecordData: function (index) {
            /// <summary>
            /// 查找并获取指定索引的行数据
            ///</summary>
            ///<param name="index" type="number">行索引</param>
            var grid = this, options = this.Options;
            if (grid.DataSource && grid.DataSource.length > 0) {
                return grid.DataSource[index];
            }
            else
                return -1;
        },
        _AppendRecord: function (index) {
            var grid = this, options = this.Options;
            var idx = grid._InSelected(index);

            if (idx == -1) {
                var row = grid._FindRecordData(index);
                grid.Selections.push(row);
            }
        },
        _RemoveRecord: function (index) {
            var grid = this, options = this.Options;
            var idx = grid._InSelected(index);
            if (idx > -1) {
                grid.Selections.splice(idx, 1);
            }
        },
        _ToggleRecord: function (index) {
            var grid = this, options = this.Options;
            var idx = grid._InSelected(index);
            if (idx == -1) {
                var row = grid._FindRecordData(index);
                grid.Selections.push(row);

                if (grid.HasBind("SelectRow"))
                    grid.Trigger("SelectRow", [index]);
            }
            if (idx > -1) {
                grid.Selections.splice(idx, 1);

                if (grid.HasBind("UnSelectRow"))
                    grid.Trigger("UnSelectRow", [index]);
            }
        },
        _InSelected: function (index) {
            /// <summary>
            /// 返回一个值，该值标识指定索引的数据行是否被选中
            ///</summary>
            ///<param name="index" type="number">行索引</param>
            var grid = this, options = this.Options;
            return $.inArray(grid._FindRecordData(index), grid.Selections);
        },
        SelectAll: function () {
            var grid = this, options = this.Options;
            if (options.CanMultiple) {
                if (options.FrozenColumns.length > 0) {
                    grid._SelectAllByFreezeTable();
                } else {
                    grid._SelectAllByGroovyTable();
                }
            }
        },
        UnSelectAll: function () {
            var grid = this, options = this.Options;
            if (options.CanMultiple) {
                if (options.FrozenColumns.length > 0) {
                    grid._UnSelectAllByFreezeTable();
                } else {
                    grid._UnSelectAllByGroovyTable();
                }
            }
        },
        _SelectAllByFreezeTable: function () {
            var grid = this, options = this.Options;
            $("tbody", grid.FreezeTable).children().each(function () {
                var row_freeze = $(this);
                var row_idx = row_freeze.attr("r_idx");
                var checkstate = options.CanShowCheckBox == true ?
                        $("#" + grid.Id + "_row_chk_" + row_idx).Frame("Options", "Checked") :
                        row_freeze.hasClass("frame-grid-state-highlight");
                if (!checkstate) {
                    var row_groovy = $("tr.frame-grid-row[r_idx=" + row_idx + "]", grid.GroovyTable);
                    grid._RowOnClick(row_idx, row_freeze, row_groovy);
                }
            });
        },
        _SelectAllByGroovyTable: function () {
            var grid = this, options = this.Options;
            $("tbody", grid.GroovyTable).children().each(function () {
                var row_groovy = $(this);
                var row_idx = row_groovy.attr("r_idx");
                if (!row_groovy.hasClass("frame-grid-state-highlight")) {
                    var row_freeze = $("tr.frame-grid-row[r_idx=" + row_idx + "]", grid.FreezeTable);
                    grid._RowOnClick(row_idx, row_freeze, row_groovy);
                }
            });
        },
        _UnSelectAllByFreezeTable: function () {
            var grid = this, options = this.Options;
            $("tbody", grid.FreezeTable).children().each(function () {
                var row_freeze = $(this);
                var row_idx = row_freeze.attr("r_idx");
                var checkstate = options.CanShowCheckBox == true ?
                        $("#" + grid.Id + "_row_chk_" + row_idx).Frame("Options", "Checked") :
                        row_freeze.hasClass("frame-grid-state-highlight");
                if (checkstate) {
                    var row_groovy = $("tr.frame-grid-row[r_idx=" + row_idx + "]", grid.GroovyTable);
                    grid._RowOnClick(row_idx, row_freeze, row_groovy);
                }
            });

        },
        _UnSelectAllByGroovyTable: function () {
            var grid = this, options = this.Options;
            $("tbody", grid.GroovyTable).children().each(function () {
                var row_groovy = $(this);
                var row_idx = row_groovy.attr("r_idx");
                if (row_groovy.hasClass("frame-grid-state-highlight")) {
                    var row_freeze = $("tr.frame-grid-row[r_idx=" + row_idx + "]", grid.FreezeTable);
                    grid._RowOnClick(row_idx, row_freeze, row_groovy);
                }
            });
        },
        GetSelected: function () {
            ///<summary>
            /// 获取当前选中记录的第一行数据行
            ///</summary>
            var grid = this, options = this.Options;
            if (grid.Selections.length > 0) {
                return grid.Selections[0];
            }

            return false;
        }
    });
})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.15
* 说明：Gritter 平铺控件。
*/
(function () {
    $.Gritter = function (options) {
        return $.Frame.Do.call(null, "Gritter", arguments, { IsStatic: true });
    };

    $.Methods.Gritter = $.Methods.Gritter || {};

    $.Defaults.Gritter = {}; // item{Title,Text,Image,Sticky,Time,Cls[info,error,success,warning,light]}

    $.Frame.Controls.Gritter = function (options) {
        $.Frame.Controls.Gritter.Parent.constructor.call(this, $("<div></div>")[0], options);
    };

    $.Frame.Controls.Gritter.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Gritter";
        },
        _IdPrev: function () {
            return "GritterFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Gritter;
        },
        _Render: function () {
            var gritter = this, options = this.Options;

            var g = $("body").find(".frame-gritter");
            if (g.length > 0)
                gritter.Element = g[0];
            else {
                $(gritter.Element).appendTo("body");
                $(gritter.Element).addClass("frame-gritter");
            }

            gritter._Custom_Timer = 0;
            gritter._Item_Count = 0;
        },
        Add: function (item) {
            var gritter = this, options = this.Options;
            gritter._Item_Count++;

            var wrapper = $("<div></div>").addClass("frame-gritter-item-wrapper")
                .attr("id", "gritter-item-wrapper" + gritter._Item_Count).attr("idx", gritter._Item_Count);

            if (item.Cls)
                wrapper.addClass("gritter-" + item.Cls);
            $(gritter.Element).append(wrapper);

            var itemcontainer = $("<div></div>").addClass("frame-gritter-item").appendTo(wrapper)
                .append($("<a></a>").addClass("frame-gritter-close").attr("idx", gritter._Item_Count).click(function () {
                    gritter.Remove($(this).attr("idx"));
                }));

            var content;
            if (item.Image) {
                var img = $("<img />").addClass("frame-gritter-image").attr("src", item.Image);
                content = $("<div></div>").addClass("frame-gritter-with-image");
                itemcontainer.append(img);
            } else {
                content = $("<div></div>").addClass("frame-gritter-without-image");
            }

            if (item.Title)
                content.append($("<span></span>").addClass("frame-gritter-title").text(item.Title || ""));
            content.append($("<p></p>").text(item.Text));
            itemcontainer.append(content);
            itemcontainer.append($("<div></div>").css("clear", "both"));
            if (item.Time)
                gritter._Custom_Timer = item.Time;

            wrapper.fadeIn("fast");

            // 在指定时间后自动移除
            if (!item.Sticky && item.Time) {
                gritter._SetFadeTimer(wrapper, gritter._Item_Count);
            }
        },
        _SetFadeTimer: function (g, d) {
            var gritter = this, options = this.Options;

            setTimeout(function () {
                gritter.Remove($(g).attr("idx"));
            },
            gritter._Custom_Timer);
        },
        Remove: function (index) {
            var gritter = this, options = this.Options;
            $(gritter.Element).children("#gritter-item-wrapper" + index).fadeOut("slow", function () {
                $(this).remove();
                if ($(gritter.Element).children().length == 0) {
                    $.Frame.Remove(gritter);
                    $(gritter.Element).remove();
                }
            });

        },
        Clear: function () {
            var gritter = this, options = this.Options;
            $(gritter.Element).empty();
            $.Frame.Remove(gritter);
        }
    });

    $.Gritter.Add = function (item) {
        var result = $.Frame.Find("Gritter");
        var gritter;
        if (result.length == 0)
            gritter = $.Gritter();
        else
            gritter = result[0];

        gritter.Add(item);
    };

    $.Gritter.Clear = function () {
        var result = $.Frame.Find("Gritter");
        var gritter;
        if (result.length == 0)
            gritter = $.Gritter();
        else
            gritter = result[0];

        gritter.Clear();
    };

})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.06.14
* 说明：Label 标签文本控件。
*/
(function ($) {
    $.fn.Label = function (options) {
        return $.Frame.Do.call(this, "Label", arguments);
    };

    $.Methods.Label = $.Methods.Label || {};

    $.Defaults.Label = {
        Text: "",
        Size: "normal", // normal,small,large,xlarge
        Mode: "primary", // info,primary,success,warning,important,error,inverse
        WithArrow: false,
        Icon: null,
        IconSite: "left", // left,right,side,only
        Width: null
    };

    $.Frame.Controls.Label = function (element, options) {
        $.Frame.Controls.Label.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Label.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Label";
        },
        _IdPrev: function () {
            return "LabelFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Label;
        },
        _PreRender: function () {
            var label = this;

            if (!$(label.Element).hasClass("frame-label")) {
                $(label.Element).addClass("frame-label");
            }
        },
        _Render: function () {
            var label = this, options = this.Options;

            options.Text = options.Text || $(label.Element).text();
            $(label.Element).empty();

            label.Set(options);
        },
        _SetText: function (text) {
            var label = this, options = this.Options;
            if (text) {
                var span = $("span", label.Element);
                if (span.length > 0) {
                    span.text(text);
                } else
                    $(label.Element).append($("<span></span>").text(text));
            }
        },
        _GetText: function () {
            var label = this, options = this.Options;
            var span = $("span", label.Element);
            if (span.length > 0)
                return span.text();
            else
                return "";
        },
        _SetSize: function (size) {
            ///<summary>
            /// 设置标签的尺寸样式
            ///</summary>
            ///<param name="size" type="string">尺寸类型.[large,xlarge,normal,samll]</param>
            var label = this, options = this.Options;
            if (size != "normal")
                $(label.Element).addClass("frame-label-" + size);
        },
        _SetMode: function (mode) {
            ///<summary>
            /// 设置按钮的类型样式
            ///</summary>
            ///<param name="mode" type="string">按钮类型.[transparent,default,primary,info,success,error,inverse,warning,important]</param>
            var label = this, options = this.Options;
            if (mode != "default") {
                $(label.Element).addClass("frame-label-" + mode);
            }
        },
        _SetWithArrow: function (arrow) {
            ///<summary>
            /// 设置标签以箭头形式展现的样式类型
            ///</summary>
            ///<param name="arrow" type="string">箭头类型.[
            ///  arrowed-left,arrowed-both,arrowed-right,
            ///  arrowed-in-left,arrowed-in-both,arrowed-in-right,
            ///  arrow-left,arrow-right]</param>
            var label = this, options = this.Options;
            if (arrow) {
                switch (arrow) {
                    case "arrowed-left": $(label.Element).addClass("arrowed"); break;
                    case "arrowed-both": $(label.Element).addClass("arrowed").addClass("arrowed-right"); break;
                    case "arrowed-right": $(label.Element).addClass("arrowed-right"); break;
                    case "arrowed-in-left": $(label.Element).addClass("arrowed-in"); break;
                    case "arrowed-in-both": $(label.Element).addClass("arrowed-in").addClass("arrowed-in-right"); break;
                    case "arrowed-in-right": $(label.Element).addClass("arrowed-in-right"); break;
                    case "arrow-left": $(label.Element).addClass("arrowed").addClass("arrowed-in-right"); break;
                    case "arrow-right": $(label.Element).addClass("arrowed-in").addClass("arrowed-right"); break;
                }
            }
        },
        _SetIcon: function (icon) {
            var label = this, options = this.Options;
            if (icon) {
                var i = $("<i></i>").addClass(icon);
                switch (options.IconSite) {
                    case "left": $(label.Element).prepend(i); break;
                    case "right": $(label.Element).append(i.addClass("icon-on-right")); break;
                    case "side":
                        var iLeft = $("<i></i>").addClass(icon[0]);
                        var iRight = $("<i></i>").addClass(icon[1]).addClass("icon-on-right");
                        $(label.Element).prepend(iLeft);
                        $(label.Element).append(iRight);
                        break;
                    case "only": $(label.Element).append(i.addClass("icon-only")); break;
                }
            }
        },
        _SetWidth: function (width) {
            var label = this, options = this.Options;
            if (width)
                $(label.Element).width(width);
        }
    });

})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.11
* 说明：Messager 消息对话框控件。
*/
(function ($) {
    $.Messager = function (options) {
        options = $.extend(options, { CloseOnDestroy: true });
//        if (self.frameElement && self.frameElement.tagName == "IFRAME") {
//            var form = $("<div></div>").FormBox(options);
//            $(form.Element).appendTo(parent.window.document.body);
//            return form;
//        } else {
//            return $.FormBox(options);
        //        }
        return $.FormBox(options);
    };

    $.Defaults.Messager = {
        Icons: {
            "info": "icon-info-sign",
            "warn": "icon-warning-sign",
            "error": "icon-remove-circle",
            "ask": "icon-question-sign",
            "exclamation": "icon-exclamation-sign"
        },
        IconColors: {
            "info": "blue",
            "warn": "orange",
            "error": "red",
            "ask": "grey",
            "exclamation": "green"
        },
        Content: function (icon, message) {
            var i = $("<i></i>").addClass(this.Icons[icon]).addClass(this.IconColors[icon]).addClass("frame-messager-icon");
            var msg = $("<p></p>").addClass("frame-messager-content").text(message);
            var content = $("<div></div>").css("display", "table").append(i).append(msg);
            return content;
        }
    };

    $.Messager.Show = function (options) {
        ///<summary>
        /// 显示一个消息窗体
        ///</summary>
        ///<param name="options" type="object">{Width,Height,Title,Message}</param>
        var content = $.Defaults.Messager.Content("info", options.Message);
        var form = $.Messager({
            Title: options.Title,
            Icon: null,
            CanDraggable: true,
            Closable: true,
            IsModal: true,
            Width: options.Width || 400,
            Height: options.Height || "auto",
            HasFooter: false,
            Content: content
        });
        form.Show();
    };

    $.Messager.Alert = function (title, message, icon, fn) {
        var content = $.Defaults.Messager.Content((icon ? icon : "info"), message);

        var form = $.Messager({
            Title: title,
            Icon: null,
            CanDraggable: true,
            Closable: false,
            IsModal: true,
            Width: 400,
            Content: content,
            Buttons: [
                { Mode: "success", Icon: "icon-ok", Text: "确定", Orientation: "right",
                    OnClick: function () {
                        if (fn) {
                            fn.call(form.Element, true);
                        }
                        if (this.Context)
                            this.Context.Destroy();
                    }
                }
            ]
        });
        form.Show();
    };

    $.Messager.Confirm = function (title, message, fn) {
        var content = $.Defaults.Messager.Content("ask", message);
        var form = $.Messager({
            Title: title,
            Icon: null,
            CanDraggable: true,
            Closable: false,
            IsModal: true,
            Width: 400,
            Content: content,
            Buttons: [
                { Mode: "success", Icon: "icon-ok", Text: "确定", Orientation: "right",
                    OnClick: function () {
                        if (fn) {
                            fn.call(form.Element, true);
                        }
                        if (this.Context)
                            this.Context.Destroy();
                    }
                },
                { Mode: "error", Icon: "icon-remove", Text: "取消",
                    OnClick: function () {
                        if (fn) {
                            fn.call(form.Element, false);
                        }
                        if (this.Context)
                            this.Context.Destroy();
                    }
                }
            ]
        });
        form.Show();
    };

})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.04
* 说明：NavBar 导航栏控件。
*/
(function ($) {
    $.fn.NavBar = function (options) {
        return $.Frame.Do.call(this, "NavBar", arguments);
    };

    $.Methods.NavBar = $.Methods.NavBar || {};

    $.Defaults.NavBar = {
        Brand: null,    // 导航栏左侧logo图标及产品名称区域:{Icon,Text}
        NavItems: [],   // 导航栏工具项 {Icon,Text,Badge,Color,DropMenu:{Header:{Icon,Title},Footer,OnLastItemClick,Children:[{Content,OnItemStyle}]}}
        UserMenu: null,  // 导航栏下拉菜单项 {Photo,Content,Items:[{Icon,Text,Divider}]}
        OnBrandClick: null,
        OnBeforeItemOpen: null,
        OnNavItemClick: null,
        OnNavItemChildClick: null,
        OnBefornUserMenuOpen: null,
        OnUserMenuItemClick: null
    };

    $.Frame.Controls.NavBar = function (element, options) {
        $.Frame.Controls.NavBar.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.NavBar.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "NavBar";
        },
        _IdPrev: function () {
            return "NavBarFor";
        },
        _ExtendMethods: function () {
            return $.Methods.NavBar;
        },
        _PreRender: function () {
            var navbar = this;

            if (!$(navbar.Element).hasClass("frame-navbar")) {
                $(navbar.Element).addClass("frame-navbar");
            }
            navbar.Children = [];
        },
        _Render: function () {
            var navbar = this, options = this.Options;

            navbar.Container = $("<div></div>").addClass("frame-navbar-container").appendTo(navbar.Element);
            navbar.BrandContainer = $("<div></div>").addClass("frame-navbar-header")
                                                    .addClass("pull-left")
                                                    .appendTo(navbar.Container);
            navbar.Brand = $("<a></a>").addClass("frame-navbar-brand").append("<small></small>").appendTo(navbar.BrandContainer);

            navbar.NavContainer = $("<div></div>").addClass("frame-navbar-header")
                                                  .addClass("pull-right")
                                                  .appendTo(navbar.Container);
            navbar.NavDropMenu = $("<ul></ul>").addClass("frame-navbar-nav").appendTo(navbar.NavContainer);

            navbar.Set(options);
        },
        _SetBrand: function (brand) {
            var navbar = this, options = this.Options;
            if (brand) {
                if (brand.Icon) {
                    var icon = $("<i></i>").addClass(brand.Icon);
                    $("small", navbar.Brand).append(icon);
                }
                $("small", navbar.Brand).append($("<span></span>").text(brand.Text));
                if (navbar.HasBind("BrandClick")) {
                    navbar.Brand.on("click", function () {
                        navbar.Trigger("BrandClick", [this])
                        return false;
                    });
                }
            }
        },
        _SetNavItems: function (navitems) {
            ///<summary>
            /// 设置导航栏列表项及下属菜单
            ///</summary>
            ///<param name="navitems" type="array">列表项对象集合,
            /// 对象结构：{Icon,Text,Badge,Color,DropMenu},
            /// 其中DropMenu为菜单项对象集合，
            /// 对象结构：{Header:{Icon,Title},Footer,OnLastItemClick,Children:[{Content,OnItemStyle}]},
            /// 内置Color可选：grey,purple,green,
            ///</param>
            var navbar = this, options = this.Options;

            $(navitems).each(function () {
                var item = navbar._CreateNavItem(this);
                navbar.NavDropMenu.append(item);
                if (this.DropMenu) {
                    navbar._CreateNavItemDropMenu(item, this.DropMenu);
                }
            });
        },
        _CreateNavItem: function (navitem) {
            ///<summary>
            /// 创建导航栏的列表项，这里不创建项所属菜单
            ///</summary>
            ///<param name="navitem" type="object">列表项对象:{Icon,Text,Badge,Color}</param>
            var navbar = this, options = this.Options;

            navbar.Children.push(navitem);

            var item = $("<li></li>").on("click", function () {
                if ($(this).hasClass("disabled"))
                    return false;
                var isOpen = true;
                var idx = navbar.NavDropMenu.children().index(this);

                // BeforeItemOpen接收参数:当前点击的元素对象,元素对象对应的数据对象,数据对象在对象集合中的索引位置
                if (navbar.HasBind("BeforeItemOpen") && !$(this).hasClass("open"))
                    isOpen = navbar.Trigger("BeforeItemOpen", [$("a.dropdown-toggle", this), navbar.Children[idx], idx]);
                if (isOpen && !$(this).hasClass("open")) {
                    $("li.open", navbar.NavDropMenu).removeClass("open");
                    $(this).addClass("open");

                    // NavItemClick接收参数:当前点击的元素对象,元素对象对应的数据对象,数据对象在对象集合中的索引位置
                    if (navbar.HasBind("NavItemClick"))
                        navbar.Trigger("NavItemClick", [$("a.dropdown-toggle", this), navbar.Children[idx], idx]);
                    return false;
                } else
                    $(this).removeClass("open");
            });
            var dropdownbutton = $("<a></a>").addClass("dropdown-toggle").appendTo(item);

            navitem.Color && item.addClass(navitem.Color);
            navitem.Icon && dropdownbutton.append($("<i></i>").addClass(navitem.Icon));

            if (navitem.Text) {
                if (!navitem.Badge)
                    dropdownbutton.append(navbar._WrapNavItemText(navitem.Text));
                else
                    dropdownbutton.append(navbar._WrapNavItemBadge(navitem.Text, navitem.Badge));
            }
            return item;
        },
        _WrapNavItemText: function (text) {
            var navbar = this, options = this.Options;

            return $("<span></span>").text(text);
        },
        _WrapNavItemBadge: function (text, badge) {
            var navbar = this, options = this.Options;

            return $("<span></span>").addClass("frame-badge").addClass("frame-badge-" + badge).text(text);
        },
        _CreateNavItemDropMenu: function (navitem, dropmenu) {
            ///<summary>
            /// 创建导航栏的工具项所属菜单
            ///</summary>
            ///<param name="navitem" type="object">工具项对象</param>
            ///<param name="dropmenu" type="object">工具项菜单的菜单项集合对象
            /// {Header:{Icon,Title},Footer:{Icon,Text},OnLastItemClick,Children:[{Content,OnItemStyle}]}</param>
            var navbar = this, options = this.Options;
            if (dropmenu.Children.length > 0) {
                var menu = $("<ul></ul>").addClass("pull-right").addClass("frame-navbar-dropdown-menu");
                var header = $("<li></li>").addClass("frame-navbar-dropdown-header").appendTo(menu);
                dropmenu.Header.Icon && $("<i></i>").addClass(dropmenu.Header.Icon).appendTo(header);
                $("<span></span>").text(dropmenu.Header.Title).appendTo(header);

                $(dropmenu.Children).each(function () {
                    var item = navbar._CreateDropMenuItem(this);
                    menu.append(item);
                });

                if (dropmenu.Footer) {
                    var last = $("<li></li>").addClass("last-child").append("<a></a>").appendTo(menu)
                        .on("click", function () {
                            if (dropmenu.OnLastItemClick) {
                                var idx = navbar.NavDropMenu.children().index(navitem[0]);

                                // LastItemClick接收参数:当前菜单所属导航栏数据对象,数据对象在导航列表中的索引位置
                                dropmenu.OnLastItemClick.call(this, navbar.Children[idx], idx);
                                return false;
                            }
                        });
                    $("a", last).append(dropmenu.Footer.Text);
                    dropmenu.Footer.Icon && $("<i></i>").addClass(dropmenu.Footer.Icon).appendTo($("a", last));
                }

                navitem.append(menu);
            }
        },
        _CreateDropMenuItem: function (dropmenuitem) {
            var navbar = this, options = this.Options;
            var item = $("<li></li>").append("<a></a>").on("click", function () {
                if ($(this).hasClass("disabled"))
                    return false;

                if (navbar.HasBind("NavItemChildClick")) {
                    var menu = $(this).parent();
                    var idx = menu.children().index(this); // 这里索引值从1起始，因为需要排除菜单标题

                    // NavItemChildClick接收参数:当前点击的元素对象,元素对象在菜单列表中的索引位置
                    navbar.Trigger("NavItemChildClick", [$(this).first(), idx]);
                }
                $("li.open", navbar.NavDropMenu).removeClass("open");
                return false;
            });
            if (dropmenuitem.OnItemStyle) {
                dropmenuitem.Content = dropmenuitem.OnItemStyle.call(this, dropmenuitem.Content);
            }
            $("a", item).append(dropmenuitem.Content);
            return item;
        },
        _SetUserMenu: function (usermenu) {
            var navbar = this, options = this.Options;

            if (usermenu) {
                var item = $("<li></li>").addClass("light-blue").addClass("usermenu").appendTo(navbar.NavDropMenu)
                .on("click", function () {
                    var isOpen = true;
                    if (navbar.HasBind("BefornUserMenuOpen"))
                        isOpen = navbar.Trigger("BefornUserMenuOpen", []);
                    if (isOpen && !$(this).hasClass("open")) {
                        $("li.open", navbar.NavDropMenu).removeClass("open");
                        $(this).addClass("open");
                    } else
                        $(this).removeClass("open");
                });
                var dropdownbutton = $("<a></a>").addClass("dropdown-toggle").appendTo(item);

                usermenu.Photo && $("<i></i>").addClass("nav-user-photo").addClass(usermenu.Photo).appendTo(dropdownbutton);
                $("<span></span>").addClass("user-info").append(usermenu.Content).appendTo(dropdownbutton);
                dropdownbutton.append($("<i></i>").addClass("icon-caret-down"));

                if (usermenu.Items.length > 0) {
                    var menu = $("<ul></ul>").addClass("pull-right").addClass("frame-dropdown-menu").appendTo(item);
                    $(usermenu.Items).each(function () {
                        if (!this.Divider) {
                            var child = $("<li></li>").append("<a></a>").appendTo(menu).on("click", function () {
                                if ($(this).hasClass("disabled"))
                                    return false;

                                if (navbar.HasBind("UserMenuItemClick"))
                                    navbar.Trigger("UserMenuItemClick", [$("span", this).text()]);
                                $("li.open", navbar.NavDropMenu).removeClass("open");
                                return false;
                            });
                            this.Icon && $("<i></i>").addClass(this.Icon).appendTo($("a", child));
                            $("a", child).append($("<span></span>").text(this.Text));
                        }
                        else
                            $("<li></li>").addClass("divider").appendTo(menu)
                    });
                }
            }
        },
        AddNavItem: function (navitem) {
            ///<summary>
            /// 追加一个导航栏列表项
            ///</summary>
            ///<param name="navitem" type="object">列表项对象:{Icon,Text,Badge,Color}</param>
            var navbar = this, options = this.Options;

            var item = navbar._CreateNavItem(navitem);

            var usermenu = $("li.usermenu", navbar.NavDropMenu);
            if (usermenu.length > 0)
                $("li.usermenu", navbar.NavDropMenu).before(item);
            else
                navbar.NavDropMenu.append(item);

            if (navitem.DropMenu) {
                navbar._CreateNavItemDropMenu(item, navitem.DropMenu);
            }

            return item;
        },
        AddNavItemDropMenu: function (index, dropmenu) {
            ///<summary>
            /// 创建导航栏的列表项所属菜单
            ///</summary>
            ///<param name="index" type="number">列表项在导航栏列表中的索引位置.索引值值从0起始.</param>
            ///<param name="dropmenu" type="object">列表项菜单的菜单项集合对象
            /// {Header:{Icon,Title},Footer:{Icon,Text},OnLastItemClick,Children:[{Content,OnItemStyle}]}</param>
            var navbar = this, options = this.Options;

            if (navbar.NavDropMenu.children().length > 0) {
                var item = navbar.NavDropMenu.children().eq(index);
                if (item.length > 0)
                    navbar._CreateNavItemDropMenu(item, dropmenu);
            }
        },
        AddNavItemDropMenuItem: function (index, dropmenuitems) {
            ///<summary>
            /// 创建导航栏的列表项所属菜单的子项
            ///</summary>
            ///<param name="index" type="number">列表项在导航栏列表中的索引位置.索引值值从0起始.</param>
            ///<param name="dropmenuitems" type="array">列表项菜单的菜单项集合对象[{Content,OnItemStyle}]</param>
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var item = navbar.NavDropMenu.children().eq(index);
                if (item.length > 0) {
                    var menu = $(".frame-navbar-dropdown-menu", item);
                    var last = $("li.last-child", menu);
                    $(dropmenuitems).each(function () {
                        var menuitem = navbar._CreateDropMenuItem(this);
                        if (last.length > 0)
                            last.before(menuitem);
                        else
                            menu.append(menuitem);
                    });
                }
            }
        },
        SetNavItemText: function (index, text) {
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var item = navbar.NavDropMenu.children().eq(index);
                if (item.length > 0) {
                    $("a>span", item).text(text);
                    navbar.Children[index].Text = text;
                }
            }
        },
        DisableNavItem: function (index) {
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var item = navbar.NavDropMenu.children().eq(index);
                if (item.length > 0)
                    item.addClass("disabled");
            }
        },
        EnableNavItem: function (index) {
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var item = navbar.NavDropMenu.children().eq(index);
                if (item.length > 0) {
                    item.removeClass("disabled");
                }
            }
        },
        DisableNavDropMenuItem: function (parent, index) {
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var item = navbar.NavDropMenu.children().eq(parent);
                if (item.length > 0) {
                    var menu = $(".frame-navbar-dropdown-menu", item);
                    if (menu.children().length > 0) {
                        var menuitem = menu.children().eq(index);
                        menuitem.length > 0 && menuitem.addClass("disabled");
                    }
                }
            }
        },
        EnableNavDropMenuItem: function (parent, index) {
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var item = navbar.NavDropMenu.children().eq(parent);
                if (item.length > 0) {
                    var menu = $(".frame-navbar-dropdown-menu", item);
                    if (menu.children().length > 0) {
                        var menuitem = menu.children().eq(index);
                        menuitem.length > 0 && menuitem.removeClass("disabled");
                    }
                }
            }
        },
        DisableUserMenu: function () {
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var menu = $(".usermenu", navbar.NavDropMenu);
                if (menu.length > 0) {
                    menu.addClass("disabled");
                }
            }
        },
        EnableUserMenu: function () {
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var menu = $(".usermenu", navbar.NavDropMenu);
                if (menu.length > 0) {
                    menu.removeClass("disabled");
                }
            }
        },
        DisableUserMenuItem: function (index) {
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var menu = $(".usermenu", navbar.NavDropMenu);
                if (menu.length > 0 && menu.children().length > 0) {
                    menu.children().eq(index).addClass("disabled");
                }
            }
        },
        EnableUserMenuItem: function (index) {
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var menu = $(".usermenu", navbar.NavDropMenu);
                if (menu.length > 0 && menu.children().length > 0) {
                    menu.children().eq(index).removeClass("disabled");
                }
            }
        }

    });


})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.06
* 说明：Navigation 菜单栏控件。
*/
(function ($) {
    $.fn.Navigation = function (options) {
        return $.Frame.Do.call(this, "Navigation", arguments);
    };

    $.Methods.Navigation = $.Methods.Navigation || {};

    $.Defaults.Navigation = {
        Items: null,       // [{Icon,Title,Badge,Children:[{Title}]}]
        Url: null,
        Params: null,
        Method: "POST",
        DataType: "json",
        AutoExpand: true,
        OnHeaderClick: null,
        OnChildirenClick: null,
        OnResolve: null,
        OnBeforeExpand: null,
        OnExpand: null
    };

    $.Frame.Controls.Navigation = function (element, options) {
        $.Frame.Controls.Navigation.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Navigation.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Navigation";
        },
        _IdPrev: function () {
            return "NavigationFor";
        },
        _ExtenMethods: function () {
            return $.Methods.Navigation;
        },
        _PreRender: function () {
            var navigation = this, options = this.Options;
            if (!$(navigation.Element).hasClass("frame-navigation")) {
                $(navigation.Element).addClass("frame-navigation");
            }

            navigation.Children = [];
        },
        _Render: function () {
            var navigation = this, options = this.Options;

            navigation.Set(options);
        },
        _SetItems: function (items) {
            var navigation = this, options = this.Options;
            navigation.Children.length = 0;

            if (items && items.length > 0) {
                $(items).each(function () {
                    navigation.Children.push(this);
                    navigation._CreateHeader(this);
                });

            }
        },
        _CreateHeader: function (item) {
            var navigation = this, options = this.Options;

            var head = $("<li></li>").appendTo(navigation.Element).on("click", function () {
                var index = $(navigation.Element).children().index(this);
                navigation.Expand(index);
                return false;
            });
            var button = $("<a></a>").appendTo(head);

            item.Icon && $("<i></i>").addClass(item.Icon).appendTo(button);
            $("<span></span>").addClass("menu-text").text(item.Title).appendTo(button);
            if (item.Children && item.Children.length > 0) {
                button.addClass("dropdown-toggle");
                $("<b></b>").addClass("arrow").addClass("icon-angle-down").appendTo(button);
                var submenu = $("<ul></ul>").addClass("submenu").appendTo(head);
                navigation._CreateChildrenItem(submenu, item.Children);
            }
        },
        _CreateChildrenItem: function (header, childs) {
            var navigation = this, options = this.Options;
            if (childs && childs.length > 0) {
                $(childs).each(function () {
                    var item = $("<li></li>").appendTo(header).on("click", function () {
                        $(".active", navigation.Element).removeClass("active");
                        var parent = $(header).parent();
                        var parentIndex = $(navigation.Element).children().index(parent[0]);
                        var index = $(header).children().index(this);
                        parent.addClass("active");
                        $(this).addClass("active");
                        if (navigation.HasBind("ChildirenClick"))
                            navigation.Trigger("ChildirenClick", [this, navigation.Children[parentIndex], navigation.Children[parentIndex].Children[index]]);
                        return false;
                    });
                    var button = $("<a></a>").append($("<i></i>").addClass("icon-double-angle-right")).appendTo(item);
                    $("<span></span>").addClass("menu-text").text(this.Title).appendTo(button);
                });
            }
        },
        Expand: function (which) {
            var navigation = this, options = this.Options;

            var index = (typeof which == "number") ? which : navigation._GetHeaderIndex(which);
            if (index < 0)
                return;

            var header = $(navigation.Element).children().eq(index);
            if ($(".submenu", header).length > 0) {
                var expand = true;
                if (navigation.HasBind("BeforeExpand"))
                    expand = navigation.Trigger("BeforeExpand", [header, navigation.Children[index]]);
                if (expand) {
                    if (options.AutoExpand) {
                        $(navigation.Element).children(".open").not(header).removeClass("open");
                    }
                    $(header).toggleClass("open");
                    if (navigation.HasBind("Expand"))
                        navigation.Trigger("Expand", [header, navigation.Children[index]]);
                }
            }
            else {
                $(".active", navigation.Element).removeClass("active");
                $(".open", navigation.Element).removeClass("open");
                $(header).addClass("active");
                if (navigation.HasBind("HeaderClick"))
                    navigation.Trigger("HeaderClick", [header, navigation.Children[index]]);
            }
        },
        _GetHeaderIndex: function (text) {
            var navigation = this, options = this.Options;
            var index = -1;
            $(options.Items).each(function (i) {
                if (this.Title == text)
                    index = i;
            });

            return index;
        },
        _SetUrl: function (url) {
            var navigation = this, options = this.Options;
            if (url != null && options.Items == null) {
                $.ajax({
                    dataType: options.DataType, type: options.Method,
                    url: url,
                    data: options.Params,
                    success: function (datas) {
                        navigation.LoadData(datas);
                    },
                    error: function (xhr, info, exp) {
                        // 三个参数:XMLHttpRequest对象、错误信息、(可选)捕获的异常对象
                    },
                    complete: function (xhr, ts) {
                    }
                });
            }
        },
        LoadData: function (datas) {
            var navigation = this, options = this.Options;
            var nodes = datas;
            if (navigation.HasBind("Resolve"))
                nodes = navigation.Trigger("Resolve", [datas]);
            navigation.Set({ Items: nodes });
        }
    });


})(jQuery);

/*
* FrameUI v2.1.0
*
* 作者：zlx
* 日期：2015.05.26
* 说明：Pagination 分页控件
*
* Copyright 2015 zlx
*
*/
(function ($) {
    $.fn.Pagination = function (options) {
        return $.Frame.Do.call(this, "Pagination", arguments);
    };

    $.Methods.Pagination = $.Methods.Pagination || {};

    $.Defaults.Pagination = {
        Total: 0,              // 总记录数，当初始化创建分页时必须设置
        PageSize: 10,          // 每页显示的记录数
        PageNumber: 1,         // 当分页创建完毕时显示当前页码
        Buttons: null,         // 自定义功能按钮组,每个按钮对象包含3个属性:Icon,Title,Color,OnClick/Separator
        OnSelectPage: false    // 当用户进行翻页时触发，回调函数包含2个参数:PageNumber,PageSize
    };

    $.Frame.Controls.Pagination = function (element, options) {
        $.Frame.Controls.Pagination.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Pagination.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Pagination";
        },
        _IdPrev: function () {
            return "PaginationFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Pagination;
        },
        _PreRender: function () {
            var pagination = this;

            // TODO：这里为了防止页面标签元素重复设置样式
            if (!$(pagination.Element).hasClass("frame-pagination")) {
                $(pagination.Element).addClass("frame-pagination");
            }

            pagination.PageTotal = 0;   // 数据表格的总页数
            pagination.CurrentPage = 0; // 记录当前页的页码
        },
        _Render: function () {
            var pagination = this, options = this.Options;
            pagination.Container = $("<div></div>").addClass("frame-pagination-control").appendTo(pagination.Element);
            pagination.Table = $("<table><tbody><tr></tr></tbody></table>").css("width", "100%")
                .addClass("frame-pagination-table").appendTo(pagination.Container);
            pagination.ToolBar = pagination._CreateToolBar();
            pagination.PageBar = pagination._CreatePageBar();
            pagination.InfPart = pagination._CreateInfPart();
            $("tr", pagination.Table).append(pagination.ToolBar).append(pagination.PageBar).append(pagination.InfPart);

            pagination.Set(options);
        },
        _CreateToolBar: function () {
            ///<summary>
            /// 创建分页控件中的工具栏
            ///</summary>
            var cell = $("<td></td>").attr("align", "left");
            var toolbar = $("<table><tbody><tr></tr></tbody></table>").addClass("frame-pagination-table").addClass("toolbar")
                                                                      .attr("border", "0")
                                                                      .attr("cellpadding", "0")
                                                                      .attr("cellspacing", "0");
            cell.append(toolbar);
            return cell;
        },
        _CreatePageBar: function () {
            ///<summary>
            /// 创建分页控件的翻页区域
            ///</summary>
            var pagination = this;
            var cell = $("<td></td>").attr("align", "center");
            var pagebar = $("<table><tbody><tr></tr></tbody></table>").addClass("frame-pagination-table").addClass("pagebar")
                                                                      .attr("border", "0")
                                                                      .attr("cellpadding", "0")
                                                                      .attr("cellspacing", "0");
            pagination.FirstButton = pagination._CreatePageButton("icon-double-angle-left");
            pagination.PrevButton = pagination._CreatePageButton("icon-angle-left");
            var input = pagination._CreateInputOnPage();
            pagination.NextButton = pagination._CreatePageButton("icon-angle-right");
            pagination.LastButton = pagination._CreatePageButton("icon-double-angle-right");

            $("tr", pagebar).append(pagination.FirstButton).append(pagination.PrevButton)
                            .append(pagination._CreateSeparatorOnPage())
                            .append(input)
                            .append(pagination._CreateSeparatorOnPage())
                            .append(pagination.NextButton).append(pagination.LastButton);
            cell.append(pagebar);

            return cell;
        },
        _CreatePageButton: function (icon) {
            ///<summary>
            /// 创建分页控件翻页区域的上一页按钮及绑定相关事件
            ///</summary>
            ///<param name="icon" type="string">按钮图标样式:[icon-double-angle-left,icon-angle-left,icon-angle-right,icon-double-angle-right].</param>
            var pagination = this, options = this.Options;
            var cell = $("<td></td>").addClass("frame-pagination-button").addClass("frame-pagination-state-disabled");
            var angle = $("<span></span>").addClass("frame-pagination-icon").addClass(icon)
                                         .addClass("bigger-140");

            cell.append(angle).on("click", function () {
                if ($(this).hasClass("frame-pagination-state-disabled"))
                    return;

                if ($("span", this).hasClass("icon-double-angle-left")) {
                    pagination._First();
                }
                if ($("span", this).hasClass("icon-angle-left")) {
                    pagination._Prev();
                }
                if ($("span", this).hasClass("icon-angle-right")) {
                    pagination._Next();
                }
                if ($("span", this).hasClass("icon-double-angle-right")) {
                    pagination._Last();
                }

            });

            return cell;
        },
        _CreateSeparatorOnPage: function () {
            ///<summary>
            /// 创建分页控件翻页区域的分割线
            ///</summary>
            var cell = $("<td></td>").addClass("frame-pagination-button").addClass("frame-pagination-state-disabled")
                                     .addClass("separator");
            var separator = $("<span></span>").addClass("frame-pagination-separator");
            cell.append(separator);

            return cell;
        },
        _CreateInputOnPage: function () {
            ///<summary>
            /// 创建分页控件翻页区域的页码输入框
            ///</summary>
            var pagination = this, options = this.Options;
            var cell = $("<td></td>").addClass("input");
            var input = $("<input />").addClass("frame-pagination-input")
                                      .attr("size", "2")
                                      .attr("maxlength", "7")
                                      .attr("type", "text").val(0).on("blur.TextBox", function () {
                                          var reg = new RegExp("^[1-9]*$");
                                          var v = $(this).val();
                                          if (!reg.test(v)) {
                                              v = pagination.CurrentPage;
                                          }
                                          pagination._GoTo(v);
                                          return false;
                                      });
            cell.append("第 ").append(input).append(" 页 &nbsp;总页数：").append("<span>0</span>");
            return cell;
        },
        _GoTo: function (page) {
            var pagination = this, options = this.Options;

            // 只有当页数>0时(即有数据),才会生效
            if (pagination.PageTotal > 0) {
                pagination.CurrentPage = page;
                pagination.Set({ PageNumber: pagination.CurrentPage });
            }
        },
        _CreateInfPart: function () {
            ///<summary>
            /// 创建分页文本显示信息区域
            ///</summary>
            var cell = $("<td></td>").attr("align", "right");
            var info = $("<div></div>").addClass("frame-pagination-info").css("text-align", "right").append("显示记录 1 - 10 &nbsp;总记录数：23");
            cell.append(info);

            return cell;
        },
        _SetTotal: function (total) {
            ///<summary>
            /// 设置表格的总记录数,计算数据的页数
            ///</summary>
            ///<param name="total" type="number">数据的总记录数.</param>
            var pagination = this, options = this.Options;
            if (total > 0) {
                var size = Math.floor((total + options.PageSize - 1) / options.PageSize); // 当前记录数可以被分为几页
                pagination.PageTotal = size;
                $(".input span", pagination.PageBar).text(size); // Page * of 后的页数 
            }
        },
        _SetPageNumber: function (num) {
            ///<summary>
            /// 设置更新数据表格当前页码
            ///</summary>
            ///<param name="num" type="number">当前页码.</param>
            var pagination = this, options = this.Options;
            var startNum = 0;
            var endNum = 0;

            pagination.CurrentPage = 0;
            if (options.Total > 0) {
                pagination.CurrentPage = num;
                startNum = ((num - 1) * options.PageSize + 1);
                endNum = num * options.PageSize;
                endNum = endNum > options.Total ? options.Total : endNum;
            }
            $(".input input.frame-pagination-input", pagination.PageBar).val(pagination.CurrentPage);
            $(".frame-pagination-info", pagination.InfPart).empty().append("显示记录 " + startNum + " - " + endNum + " &nbsp;总记录数：" + options.Total);

            pagination._PageButtonChange();

            if (pagination.HasBind("SelectPage") && pagination.CurrentPage > 0) {
                pagination.Trigger("SelectPage", [pagination.CurrentPage, options.PageSize]);
            }
        },
        _PageButtonChange: function () {
            ///<summary>
            /// 设置翻页按钮及页码输入文本框的禁用状态
            ///</summary>
            var pagination = this, options = this.Options;

            if (pagination.CurrentPage == 1 && pagination.CurrentPage == pagination.PageTotal) {
                // 当前位于首页，且页数只有一页，4个翻页按钮都禁用
                pagination._DisablePageButton("FirstButton");
                pagination._DisablePageButton("PrevButton");
                pagination._DisablePageButton("NextButton");
                pagination._DisablePageButton("LastButton");
            }
            if (pagination.CurrentPage == 1 && pagination.CurrentPage < pagination.PageTotal) {
                // 当前位于首页，且页数不止一页，下一页及尾页翻页按钮可用
                pagination._DisablePageButton("FirstButton");
                pagination._DisablePageButton("PrevButton");
                pagination._EnablePageButton("NextButton");
                pagination._EnablePageButton("LastButton");
            }
            if (pagination.CurrentPage > 1 && pagination.CurrentPage < pagination.PageTotal) {
                // 当前位于中间区间，4个翻页按钮可用
                pagination._EnablePageButton("FirstButton");
                pagination._EnablePageButton("PrevButton");
                pagination._EnablePageButton("NextButton");
                pagination._EnablePageButton("LastButton");
            }
            if (pagination.CurrentPage > 1 && pagination.CurrentPage == pagination.PageTotal) {
                // 当前位于尾页，下一页及尾页翻页按钮禁用
                pagination._EnablePageButton("FirstButton");
                pagination._EnablePageButton("PrevButton");
                pagination._DisablePageButton("NextButton");
                pagination._DisablePageButton("LastButton");
            }

        },
        _EnablePageButton: function (key) {
            var pagination = this;
            if (pagination[key].hasClass("frame-pagination-state-disabled"))
                pagination[key].removeClass("frame-pagination-state-disabled");
        },
        _DisablePageButton: function (key) {
            var pagination = this;
            if (!pagination[key].hasClass("frame-pagination-state-disabled"))
                pagination[key].addClass("frame-pagination-state-disabled");
        },
        _First: function () {
            var pagination = this;
            pagination.CurrentPage = 1;
            pagination.Set({ PageNumber: 1 });
        },
        _Prev: function () {
            var pagination = this;
            if (pagination.CurrentPage > 1)
                pagination.CurrentPage--;
            pagination.Set({ PageNumber: pagination.CurrentPage });
        },
        _Next: function () {
            var pagination = this;
            if (pagination.CurrentPage < pagination.PageTotal)
                pagination.CurrentPage++;
            pagination.Set({ PageNumber: pagination.CurrentPage });
        },
        _Last: function () {
            var pagination = this;
            pagination.CurrentPage = pagination.PageTotal;
            pagination.Set({ PageNumber: pagination.PageTotal });
        },
        _SetButtons: function (buttons) {
            var pagination = this, options = this.Options;
            if (buttons && buttons.length > 0) {
                $(buttons).each(function (i) {
                    var cell = $("<td></td>").addClass("frame-pagination-button");
                    if (!this.Separator) {
                        var button = $("<div></div>").attr("idx", i).addClass("frame-pagination-div").appendTo(cell)
                        .on("click", function () {
                            var idx = $(this).attr("idx");
                            if (buttons[idx].Click) {
                                buttons[idx].Click.call(this, buttons[idx].Title);
                            }
                            return false;
                        });
                        this.Color = this.Color || "grey";
                        var icon = $("<span></span>").addClass("frame-pagination-icon").addClass(this.Icon).addClass(this.Color)
                                                     .attr("tip", this.Title).appendTo(button);
                        this.Title && $(icon).Tip({ Placement: "auto", Mode: "inverse" });
                    }
                    else {
                        cell.addClass("separator").append($("<span></span>").addClass("frame-pagination-separator"));
                    }
                    $(".toolbar tr", pagination.ToolBar).append(cell);
                });
            }
        },
        Refresh: function (total, pagenumber) {
            ///<summary>
            /// 根据总记录数和当前页码,刷新分页的页数
            ///</summary>
            ///<param name="total" type="number">数据的总记录数。</param>
            ///<param name="pagenumber" type="number">当前设置的页码。</param>
            var pagination = this, options = this.Options;
            pagination.Set({ Total: total, PageNumber: pagenumber });
        }
    });

})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.10
* 说明：Panel 面板控件。
*/
(function ($) {
    $.fn.Panel = function (options) {
        return $.Frame.Do.call(this, "Panel", arguments);
    };

    $.Methods.Panel = $.Methods.Panel || {};

    $.Defaults.Panel = {
        HasHeader: false,
        Title: "无标题",
        Icon: "icon-desktop",
        ToolBar: [],      // {Icon,OnClick}
        HasFooter: false,
        Commands: [],    // Button
        Width: null,
        Height: null,
        Plain: false,    // 简洁效果，即不带边框
        Url: null,
        Params: {},
        Content: null
    };

    $.Frame.Controls.Panel = function (element, options) {
        $.Frame.Controls.Panel.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Panel.Extension($.Frame.Controls.WindowBase, {
        _GetType: function () {
            return "Panel";
        },
        _IdPrev: function () {
            return "PanelFor";
        },
        _ExtenMethods: function () {
            return $.Methods.Panel;
        },
        _PreRender: function () {
            var panel = this, options = this.Options;
            if (!$(panel.Element).hasClass("frame-panel")) {
                $(panel.Element).addClass("frame-panel");
            }

            if ($(panel.Element).children().length > 0) {
                options.Content = $(panel.Element).children().first().detach();
                $(panel.Element).empty();
            }
        },
        _Render: function () {
            var panel = this, options = this.Options;

            panel.Header = $("<div></div>").addClass("frame-panel-header");
            panel.Container = $("<div></div>").addClass("frame-panel-container");
            panel.Inner = $("<div></div>").addClass("frame-panel-inner").appendTo(panel.Container);
            panel.Content = $("<div></div>").addClass("frame-panel-main").appendTo(panel.Inner);
            panel.Footer = $("<div></div>").addClass("frame-panel-toolbox").addClass("clearfix")
                                           .append("<hr />").appendTo(panel.Inner);

            $(panel.Element).append(panel.Header).append(panel.Container);

            panel.TimerId = null;
            panel.Set(options);
        },
        _SetHasHeader: function (hasHeader) {
            var panel = this, options = this.Options;
            if (!hasHeader) {
                panel.Header.remove();
            }
        },
        _SetTitle: function (title) {
            var panel = this, options = this.Options;
            if (title && options.HasHeader) {
                $("<h5></h5>").addClass("lighter").append($("<span></span>").text(title)).appendTo(panel.Header);
            }
        },
        _SetIcon: function (icon) {
            var panel = this, options = this.Options;
            if (options.HasHeader) {
                $("h5", panel.Header).prepend($("<i></i>").addClass(icon));
            }
        },
        _SetToolBar: function (tools) {
            var panel = this, options = this.Options;
            if (options.HasHeader && tools && tools.length > 0) {
                var bar = $("<div></div>").addClass("frame-panel-toolbar").appendTo(panel.Header);
                $(tools).each(function () {
                    var tool = this;
                    var btn = $("<a></a>").append($("<i></i>").addClass(tool.Icon)).appendTo(bar);
                    if (tool.OnClick) {
                        btn.on("click", function () {
                            tool.OnClick.call(this, panel);
                        });
                    }
                });
            }
        },
        _SetHasFooter: function (hasFooter) {
            var panel = this, options = this.Options;
            if (!hasFooter) {
                panel.Footer.remove();
            }
        },
        _SetCommands: function (commands) {
            var panel = this, options = this.Options;
            if (options.HasFooter && commands && commands.length > 0) {
                $(commands).each(function () {
                    var button = $("<button></button>").Button(this);
                    var orientation = this.Orientation == "right" ? "pull-right" : "pull-left";
                    panel.Footer.append($(button.Element).addClass(orientation));
                });
            }
        },
        _SetContent: function (content) {
            var panel = this, options = this.Options;
            if (content) {
                panel.Content.append($(content));
            }
        },
        _SetUrl: function (url) {
            var panel = this, options = this.Options;
            if (url) {
                if (options.Params) {
                    url += url.indexOf('?') == -1 ? "?" : "&";
                    for (var name in options.Params) {
                        url += (name + "=" + options.Params[name] + "&");
                    }
                    url += ("random=" + new Date().getTime());
                }
                var iframe = $("iframe", panel.Content);
                if (iframe.length > 0) {
                    iframe.attr("src", url);
                } else {
                    var framename = "Frame" + new Date().getTime();
                    iframe = $('<iframe onLoad="$.iFrameResize(this);"></iframe>').attr("id", framename).attr("name", framename)
                                                                               .attr("width", "100%").attr("frameborder", "0")
                                                                               .attr("scrolling", "no").attr("src", url)
                                                                               .appendTo(panel.Content);

                    // TODO：这里使用定时循环监控iframe中的页面高度，当高度有变时重置iframe高度
                    if (panel.TimerId)
                        window.clearInterval(panel.TimerId);
                    panel.TimerId = setInterval(function () { $.iFrameResize(iframe[0]); }, 250);
                }
            }
        },
        _SetWidth: function (width) {
            var panel = this, options = this.Options;
            if (width)
                $(panel.Element).width(width);
        },
        _SetHeight: function (height) {
            var panel = this, options = this.Options;
            if (height) {
                var head = options.HasHeader ? panel.Header.outerHeight(true) : 0;
                var foot = options.HasFooter ? panel.Footer.outerHeight(true) : 0;
                height = height - head - foot - 24;
                panel.Content.height(height);
            }
        },
        _SetPlain: function (plain) {
            var panel = this, options = this.Options;
            if (plain) {
                $(panel.Element).addClass("no-border");
            }
        },
        Load: function (url, params) {
            var panel = this, options = this.Options;
            if (params)
                options.Params = $.extend(options.Params, params);

            panel.Set({ Url: url });
        },
        Reload: function (params) {
            var panel = this, options = this.Options;
            var url = options.Url;
            panel.Load(url, params);
        }
    });


})(jQuery);


/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.06.29
* 说明：Popover 弹窗提示控件。
*       TODO：Popover控件无法使用$("xxx").Frame()来调用FrameUI实例对象,而是使用$("xxx").Popover()
*/
(function ($) {
    $.Popover = function (options) {
        return $.Frame.Do.call(options.Target, "Popover", arguments, options);
    };

    $.fn.Popover = function (options) {
        options = $.extend({ IdAttr: "framepopoverid",
            HasElement: false
        }, $.Defaults.Popover, options || {});

        options.Target = options.Target || $(this);

        if ($(this).attr("framepopoverid"))
            return $.Frame.Get(this, "framepopoverid");
        else
            return $.Popover(options);
    };

    $.Methods.Popover = $.Methods.Popover || {};

    $.Defaults.Popover = {
        Width: null,
        Mode: null,      //[error,success,warning,info,notitle]
        Content: null,
        Title: null,
        Placement: "auto"
    };

    $.Frame.Controls.Popover = function (options) {
        $.Frame.Controls.Popover.Parent.constructor.call(this, null, options);
    };

    $.Frame.Controls.Popover.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Popover";
        },
        _IdPrev: function () {
            return "PopoverFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Popover;
        },
        _Render: function () {
            var popover = this, options = this.Options;
            options.Target.attr(options.IdAttr, popover.Id);

            popover.Element = $("<div></div>").addClass("frame-popover").appendTo("body")[0];
            $("<div></div>").addClass("arrow").appendTo(popover.Element);
            $("<h3></h3>").addClass("frame-popover-title").appendTo(popover.Element);
            $("<div></div>").addClass("frame-popover-content").appendTo(popover.Element);

            popover.Set(options);
        },
        _SetWidth: function (width) {
            var popover = this, options = this.Options;
            if (width) {
                $(popover.Element).width(width);
            }
        },
        _SetMode: function (mode) {
            var popover = this, options = this.Options;
            if (mode) {
                $(popover.Element).addClass("frame-popover-" + mode);
            }
        },
        _SetTitle: function (title) {
            var popover = this, options = this.Options;
            var caption = ("function" == typeof title ? title.call(popover.Element) : title);
            $(".frame-popover-title", popover.Element).append(caption);
        },
        _SetContent: function (content) {
            var popover = this, options = this.Options;
            var detail = ("function" == typeof content ? content.call(popover.Element) : content);
            $(".frame-popover-content", popover.Element).append(detail);
        },
        _InitPlacement: function (placement) {
            var popover = this, options = this.Options;
            var reg = /\s?auto?\s?/i;
            var r = reg.test(placement);
            $(popover.Element).removeClass("in top bottom left right");

            // 当placement为auto时，置换为top
            r && (placement = placement.replace(reg, "") || "top");

            $(popover.Element).css({ top: 0, left: 0 }).addClass(placement);
            var position = popover._GetPosition();
            var w = $(popover.Element).outerWidth(true);
            var h = $(popover.Element).outerHeight(true);

            if (r) {
                var t = placement;
                var ww = $(window).width();
                var wh = $(window).height();
                var stop = document.documentElement.scrollTop || document.body.scrollTop;
                var sleft = document.documentElement.scrollLeft || document.body.scrollLeft;
                if ("bottom" == t && position.top + position.height + h - stop > wh)
                    placement = "top";
                else if ("top" == t && position.top - stop - h < 0)
                    placement = "bottom";
                else if ("right" == t && position.right + w > ww)
                    placement = "left";
                else if ("left" == t && position.left - w < 0)
                    placement = "right";
                else
                    placement = t;
                $(popover.Element).removeClass(t).addClass(placement);
            }
            var offset = popover._GetCalculatedOffset(placement, position, w, h);
            popover._ApplyPlacement(offset, placement);
        },
        _ApplyPlacement: function (offset, placement) {
            var popover = this, options = this.Options;
            var c;
            var w = $(popover.Element).outerWidth(true);
            var h = $(popover.Element).outerHeight(true);
            var margin_top = parseInt($(popover.Element).css("margin-top"), 10);
            var margin_left = parseInt($(popover.Element).css("margin-left"), 10);

            isNaN(margin_top) && (margin_top = 0);
            isNaN(margin_left) && (margin_left = 0);
            offset.top = offset.top + margin_top;
            offset.left = offset.left + margin_left;

            $(popover.Element).offset(offset);
            var nw = $(popover.Element).outerWidth(true);
            var nh = $(popover.Element).outerHeight(true);

            if ("top" == placement && nh != h && (c = !0, offset.top = offset.top + h - nh), /bottom|top/.test(placement)) {
                var k = 0;
                offset.left < 0 && (k = -2 * offset.left, offset.left = 0, $(popover.Element).offset(offset));
            }
            c && $(popover.Element).offset(offset);
        },
        _GetPosition: function () {
            var popover = this, options = this.Options;
            var target = options.Target;
            return $.extend({}, "function" == typeof target[0].getBoundingClientRect ? target[0].getBoundingClientRect() : {
                width: target[0].offsetWidth,
                height: target[0].offsetHeight
            }, target.offset());
        },
        _GetCalculatedOffset: function (placement, position, w, h) {
            switch (placement) {
                case "bottom":
                    return {
                        top: position.top + position.height,
                        left: position.left + position.width / 2 - w / 2
                    };
                case "top":
                    return {
                        top: position.top - h,
                        left: position.left + position.width / 2 - w / 2
                    };
                case "left":
                    return {
                        top: position.top + position.height / 2 - h / 2,
                        left: position.left - w
                    };
                default:
                    return {
                        top: position.top + position.height / 2 - h / 2,
                        left: position.left + position.width
                    };
            }
        },
        Show: function () {
            var popover = this, options = this.Options;
            popover._InitPlacement(options.Placement);
            $(popover.Element).show();
        },
        Close: function () {
            var popover = this, options = this.Options;
            $(popover.Element).hide();
        },
        IsVisible: function () {
            var popover = this;
            return $(popover.Element).is(":visible");
        },
        Toggle: function () {
            var popover = this, options = this.Options;
            if ($(popover.Element).is(":visible") == false) {
                popover.Show();
            } else {
                popover.Close();
            }
        },
        Reset: function (content) {
            ///<summary>
            /// 重置提示的文本内容
            ///</summary>
            ///<param name="content" type="string">提示内容文本</param>
            var popover = this, options = this.Options;
            $(".frame-popover-content", popover.Element).empty();
            popover.Set({ Content: content });
        }
    });

})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.06.20
* 说明：Radio 单选框控件。
*/
(function ($) {
    $.fn.Radio = function (options) {
        return $.Frame.Do.call(this, "Radio", arguments);
    };

    $.Methods.Radio = $.Methods.Radio || {};

    $.Defaults.Radio = {
        Value: null,
        Checked: null,
        Disabled: false,
        OnChange: null
    };

    $.Frame.Controls.Radio = function (element, options) {
        $.Frame.Controls.Radio.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Radio.Extension($.Frame.Controls.Base, {
        _GetType: function () {
            return "Radio";
        },
        _IdPrev: function () {
            return "RadioFor";
        },
        _ExtenMethods: function () {
            return $.Methods.Radio;
        },
        _PreRender: function () {
            var radio = this, options = this.Options;

            if (!$(radio.Element).hasClass("frame-radio")) {
                $(radio.Element).addClass("frame-radio");
            }

            options.Value = options.Value || $(radio.Element).val();
            options.Checked = options.Checked || $(radio.Element)[0].checked;
        },
        _Render: function () {
            var radio = this, options = this.Options;

            var parent = $(radio.Element).parent();

            radio.Container = $(radio.Element).wrap("<label></label>").parent();
            radio.Label = $("<span></span>").addClass("lbl").insertAfter(radio.Element);
            radio.Wrapper = $(radio.Element).parent().wrap($("<div></div>").addClass("frame-checkbox-wrapper")).parent();

            $(radio.Element).on("change", function (v) {
                if (radio.HasBind("Change"))
                    radio.Trigger("Change", [v.target.checked]);
                return false;
            });

            radio.Set(options);
        },
        _SetValue: function (value) {
            var radio = this, options = this.Options;
            if (value)
                radio.Label.text(value);
        },
        _GetValue: function () {
            var radio = this, options = this.Options;
            return radio.Label.text();
        },
        _SetDisabled: function (disabled) {
            ///<summary>
            /// 设置单选框禁用状态
            ///</summary>
            ///<param name="disabled" type="bool">设置一个值,该值标识复选框是否被禁用或启用[false:启用,true:禁用].</param>
            var radio = this, options = this.Options;
            if (disabled)
                $(radio.Element).attr("disabled", disabled);
            else
                $(radio.Element).removeAttr("disabled");
        },
        _GetDisabled: function () {
            ///<summary>
            /// 获取单选框状态
            ///</summary>
            var radio = this, options = this.Options;
            return $(radio.Element).attr("disabled");
        },
        Status: function () {
            var radio = this, options = this.Options;
            return radio._GetChecked();
        },
        _SetChecked: function (checked) {
            ///<summary>
            /// 设置单选框勾选状态
            ///</summary>
            ///<param name="checked" type="bool">设置一个值,该值标识是否复选框被勾选.</param>
            var radio = this, options = this.Options;
            $(radio.Element)[0].checked = checked;
        },
        _GetChecked: function () {
            ///<summary>
            /// 获取单选框勾选状态
            ///</summary>
            var radio = this, options = this.Options;
            return $(radio.Element)[0].checked
        },
        Checked: function (checked) {
            var radio = this, options = this.Options;
            var disabled = $(radio.Element).attr("disabled");
            if (disabled)
                return;
            radio.Set({ Checked: checked });
            $(radio.Element).trigger("change");
        }
    });

})(jQuery);

/**
* jQuery FrameUI 1.2.2
* 
* 作者：zlx
* 日期：2014.08.11
* 说明：元素尺寸调整插件。 
*
*/
(function ($) {
    $.fn.Resizable = function (options) {
        options = $.extend({
            IdAttr: "FrameResizableId",
            HasElement: false,
            PropertyToElement: "Target"
        }, options || {});
        return $.Frame.Do.call(this, "Resizable", arguments, options);
    };

    $.Defaults.Resizable = {
        Handles: "n, e, s, w, ne, se, sw, nw", // 调整尺寸拉动方向句柄
        MaxWidth: 2000,
        MaxHeight: 2000,
        MinWidth: 20,
        MinHeight: 20,
        Scope: 3,
        Disabled: false,
        Animate: false,
        OnStartResize: null,
        OnResize: null,
        OnStopResize: null,
        OnEndResize: null
    };

    $.Frame.Controls.Resizable = function (options) {
        $.Frame.Controls.Resizable.Parent.constructor.call(this, null, options);
    };

    $.Frame.Controls.Resizable.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Resizable";
        },
        _IdPrev: function () {
            return "ResizableFor";
        },
        _PreRender: function () {
            this.Options.Target.FrameResizableId = this.Id;
            if (!$(this.Options.Target).hasClass("frame-resizable"))
                $(this.Options.Target).addClass("frame-resizable");
            $.extend(this, {
                Target: $(this.Options.Target),
                Orientation: null,
                Proxy: null,
                Handles: null,
                Disabled: false,
                Current: null,
                // ChangeBy:标识光标调整目标元素大小时所拉动的方向所对应要改变的位置或大小变量
                ChangeBy: {
                    t: ["n", "ne", "nw"],  // 当Orientation的值包括在数组其中，标识需要重置目标元素的top
                    l: ["w", "sw", "nw"],  // 当Orientation的值包括在数组其中，标识需要重置目标元素的left
                    w: ["w", "sw", "nw", "e", "ne", "se"], // 当Orientation的值包括在数组其中，标识需要重置目标元素的宽度
                    h: ["n", "ne", "nw", "s", "se", "sw"]  // 当Orientation的值包括在数组其中，标识需要重置目标元素的高度
                }
            });
        },
        _Render: function () {
            var resizable = this, options = this.Options;
            resizable.Set(options);

            resizable.Target.mousemove(function (e) {
                if (resizable.Disabled) return;

                // 根据鼠标的位置，设置鼠标的显示样式
                resizable.Set("Orientation", e);

                // 判断目标元素是否同时应用拖动插件
                if (options.Target.FrameDraggableId) {
                    var draggable = $.Frame.Get(options.Target.FrameDraggableId);
                    if (draggable && resizable.Orientation) {
                        // 若鼠标停留在调整目标元素尺寸的位置，则禁用拖动
                        draggable.Disable();
                    }
                    else if (draggable) {
                        // 非调整尺寸大小，则启用拖动
                        draggable.Enable();
                    }
                }

            })
            .mousedown(function (e) {
                if (resizable.Disabled) return;
                if (resizable.Orientation) {
                    resizable._Start(e);
                }
            });
        },
        _SetHandles: function (handles) {
            ///<summary>
            /// 设置调整元素尺寸时光标方向句柄
            ///</summary>
            ///<param name="handles" type="string">方向句柄，默认为n, e, s, w, ne, se, sw, nw</param>
            if (!handles) return;

            // 将句柄字符串通过，分割成数组
            this.Handles = handles.replace(/(\s*)/g, "").split(",");
        },
        _SetDisabled: function (value) {
            if (!value) {
                this.Disabled = false;
                return;
            }
            this.Disabled = value;
        },
        _GetDisabled: function () {
            return this.Disabled;
        },
        _SetOrientation: function (e) {
            ///<summary>
            /// 设置光标方向样式
            ///</summary>
            ///<param name="e" type="object">鼠标事件对象</param>
            var resizable = this, options = this.Options;
            var orientation = "";

            var xy = resizable.Target.offset();
            var width = resizable.Target.width();
            var height = resizable.Target.height();
            var scope = options.Scope;

            var pageX = e.pageX || e.screenX;
            var pageY = e.pageY || e.screenY;

            if (pageY >= xy.top && pageY < xy.top + scope) {
                orientation += "n"; // 向上
            }
            else if (pageY <= xy.top + height && pageY > xy.top + height - scope) {
                orientation += "s"; // 向下
            }

            if (pageX >= xy.left && pageX < xy.left + scope) {
                orientation += "w"; // 向右
            }
            else if (pageX <= xy.left + width && pageX > xy.left + width - scope) {
                orientation += "e"; // 向左
            }

            if (options.Handles == "all" || orientation == "") {
                resizable.Orientation = orientation;
            }
            else if ($.inArray(orientation, resizable.Handles) != -1) {
                resizable.Orientation = orientation;
            }
            else
                resizable.Orientation = "";

            // 设置光标的样式
            if (resizable.Orientation)
                resizable.Target.css("cursor", resizable.Orientation + "-resize");
            else if (resizable.Target.css("cursor").indexOf("-resize") > 0)
                resizable.Target.css("cursor", "default");
        },
        _CreateProxy: function () {
            return $("<div></div>").css("display", "none")
                                    .css("overflow", "hidden")
                                    .css("border-style", "dashed")
                                    .css("border-width", "1px")
                                    .css("opacity", "0.5")
                                    .css("filter", "alpha(opacity=50)")
                                    .css("position", "absolute")
                                    .css("top", "0")
                                    .css("left", "0")
                                    .css("z-index", "10002")
                                    .css("background", "#f2f1f1");
        },
        _RemoveProxy: function () {
            var resizable = this;
            if (resizable.Proxy) {
                resizable.Proxy.remove();
                resizable.Proxy = null;
            }
        },
        _SetProxy: function (cursor) {
            ///<summary>
            /// 设置代理元素对象
            ///</summary>
            ///<param name="cursor" type="string">光标样式</param>
            var resizable = this;
            if (!cursor) {
                resizable.Proxy = resizable._CreateProxy();
                resizable.Proxy.width(resizable.Target.width()).height(resizable.Target.height());
                resizable.Proxy.appendTo("body");
                resizable.Proxy.css({
                    top: resizable.Target.offset().top,
                    left: resizable.Target.offset().left
                });
            }
            else {
                resizable.Proxy.css("cursor", cursor);
            }
        },
        _SetCurrent: function (e) {
            var resizable = this;
            resizable.Current = {
                Orientation: resizable.Orientation,
                Top: resizable.Target.offset().top,
                Left: resizable.Target.offset().left,
                StartX: e.pageX || e.screenX,
                StartY: e.pageY || e.screenY,
                Width: resizable.Target.width(),
                Height: resizable.Target.height(),
                DiffX: 0,
                DiffY: 0,
                DiffLeft: 0,
                DiffTop: 0,
                NewWidth: 0,
                NewHeight: 0
            };
        },
        _Start: function (e) {
            ///<summary>
            /// 开始调整目标元素的尺寸大小
            ///</summary>
            ///<param name="e" type="object">鼠标事件对象</param>
            var resizable = this, options = this.Options;

            resizable._RemoveProxy();
            // 创建目标元素的代理对象
            resizable.Set("Proxy");

            // 调用_SetCurrent，初始化位置坐标等原始数据，用于之后调整元素尺寸时计算使用
            resizable.Set("Current", e);

            if (resizable.HasBind("StartResize"))
                resizable.Trigger("StartResize", [resizable.Current, e]);

            $(document).bind("selectstart.Resizable", function () { return false; });
            $(document).bind("mousemove.Resizable", function () {
                resizable._Resize.apply(resizable, arguments);
            });
            $(document).bind("mouseup.Resizable", function () {
                resizable._Stop.apply(resizable, arguments);
            });
            resizable.Proxy.show();
        },
        _Resize: function (e) {
            ///<summary>
            /// 调整目标元素的尺寸大小
            ///</summary>
            ///<param name="e" type="object">鼠标事件对象</param>
            var resizable = this, options = this.Options;
            if (!resizable.Current) return;
            if (!resizable.Proxy) return;

            resizable.Set("Proxy", resizable.Orientation);

            var pageX = e.pageX || e.screenX;
            var pageY = e.pageY || e.screenY;
            resizable.Current.DiffX = pageX - resizable.Current.StartX;
            resizable.Current.DiffY = pageY - resizable.Current.StartY;

            // 调整目标元素的代理对象的尺寸大小
            resizable._ApplyResize(resizable.Proxy);

            if (resizable.HasBind("Resize"))
                resizable.Trigger("Resize", [resizable.Current, e]);
        },
        _Stop: function (e) {
            ///<summary>
            /// 结束调整目标元素的尺寸大小
            ///</summary>
            ///<param name="e" type="object">鼠标事件对象</param>
            var resizable = this, options = this.Options;

            if (resizable.HasBind("StopResize")) {
                if (resizable.Trigger("StopResize", [resizable.Current, e]) != false)
                    resizable._ApplyResize();
            }
            else {
                resizable._ApplyResize();
            }

            resizable._RemoveProxy();
            if (resizable.HasBind("EndResize"))
                resizable.Trigger("EndResize", [e]);
            $(document).unbind("selectstart.Resizable");
            $(document).unbind("mousemove.Resizable");
            $(document).unbind("mouseup.Resizable");
        },
        _ApplyResize: function (applyResultBody) {
            ///<summary>
            /// 改变目标元素的尺寸大小
            ///</summary>
            ///<param name="applyResultBody" type="selector">目标元素的代理对象</param>
            var resizable = this, options = this.Options;

            var position = resizable._CreatePosition(applyResultBody);
            var applyToTarget = false;
            if (!applyResultBody) {
                applyResultBody = resizable.Target;
                applyToTarget = true;
            }

            if ($.inArray(resizable.Current.Orientation, resizable.ChangeBy.l) > -1) {
                // 这里判断(目标元素的宽度-调整尺寸偏移量)之后得到的数值结果是否小于允许的最小宽度值，即由左向右调整缩小目标元素大小
                // 若小于最小宽度值，那么目标元素的偏移坐标left=原left坐标值+(原宽度值-最小宽度值)，这里因为元素宽度将会为最小宽度
                if (resizable.Current.Width - resizable.Current.DiffX < options.MinWidth)
                    position.left = resizable.Current.Left + (resizable.Current.Width - options.MinWidth);
                else
                    position.left += resizable.Current.DiffX;
                resizable.Current.DiffLeft = resizable.Current.DiffX;

            }
            else if (applyToTarget) {
                delete position.left;
            }

            if ($.inArray(resizable.Current.Orientation, resizable.ChangeBy.t) > -1) {
                if (resizable.Current.Height - resizable.Current.DiffY < options.MinHeight)
                    position.top = resizable.Current.Top + (resizable.Current.Height - options.MinHeight);
                else
                    position.top += resizable.Current.DiffY;
                resizable.Current.DiffTop = resizable.Current.DiffY;
            }
            else if (applyToTarget) {
                delete position.top;
            }

            if ($.inArray(resizable.Current.Orientation, resizable.ChangeBy.w) > -1) {
                position.width += (resizable.Current.Orientation.indexOf("w") == -1 ? 1 : -1) * resizable.Current.DiffX;
                if (position.width < options.MinWidth)
                    position.width = options.MinWidth;
                if (position.width > options.MaxWidth)
                    position.width = options.MaxWidth;
                resizable.Current.NewWidth = position.width;
            }
            else if (applyToTarget) {
                delete position.width;
            }

            if ($.inArray(resizable.Current.Orientation, resizable.ChangeBy.h) > -1) {
                position.height += (resizable.Current.Orientation.indexOf("n") == -1 ? 1 : -1) * resizable.Current.DiffY;

                if (position.height < options.MinHeight)
                    position.height = options.MinHeight;
                if (position.height > options.MaxHeight)
                    position.height = options.MaxHeight;
                resizable.Current.NewHeight = position.height;
            }
            else if (applyToTarget) {
                delete position.height;
            }

            if (applyToTarget && options.Animate)
                applyResultBody.animate(position);
            else
                applyResultBody.css(position);
        },
        _CreatePosition: function (applyToTarget) {
            var resizable = this, options = this.Options;

            var applyToCurrent = {
                top: resizable.Current.Top,
                left: resizable.Current.Left,
                width: resizable.Current.Width,
                height: resizable.Current.Height
            };

            if (!applyToTarget) {
                if (!isNaN(parseInt(resizable.Target.css("top"))))
                    applyToCurrent.top = parseInt(resizable.Target.css("top"));
                else
                    applyToCurrent.top = 0;
                if (!isNaN(parseInt(resizable.Target.css("left"))))
                    applyToCurrent.left = parseInt(resizable.Target.css("left"));
                else
                    applyToCurrent.left = 0;
            }

            return applyToCurrent;
        },
        Enable: function () {
            this.Disabled = false;
            this.Options.Disabled = false;
        },
        Disable: function () {
            this.Disabled = true;
            this.Options.Disabled = true;
            this.Orientation = "";
            this.Target.css("cursor", "default");
        }
    });
})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.05
* 说明：SideBar 侧边栏控件。
*/
(function ($) {
    $.fn.SideBar = function (options) {
        return $.Frame.Do.call(this, "SideBar", arguments);
    };

    $.Methods.SideBar = $.Methods.SideBar || {};

    $.Defaults.SideBar = {
        Shortcuts: null,   // 快捷键 [{Icon,Mode,OnClick}]
        Content: null       // 侧边栏中包裹的内容,这里可以一个function函数返回jquery标签对象或者字符串表示的标签
    };

    $.Frame.Controls.SideBar = function (element, options) {
        $.Frame.Controls.SideBar.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.SideBar.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "SideBar";
        },
        _IdPrev: function () {
            return "SideBarFor";
        },
        _ExtendMethods: function () {
            return $.Methods.SideBar;
        },
        _PreRender: function () {
            var sidebar = this, options = this.Options;

            if (!$(sidebar.Element).hasClass("frame-sidebar")) {
                $(sidebar.Element).addClass("frame-sidebar");
            }

            if ($(sidebar.Element).children().length > 0) {
                options.Content = $(sidebar.Element).children().first().detach();
                $(sidebar.Element).empty();
            }
        },
        _Render: function () {
            var sidebar = this, options = this.Options;
            sidebar.Set(options);

            sidebar.Collapse = $("<i></i>").addClass("icon-double-angle-left")
            .on("click", function () {
                if ($(this).hasClass("icon-double-angle-left")) {
                    $(sidebar.Element).addClass("min-sidebar");
                    $(this).removeClass("icon-double-angle-left").addClass("icon-double-angle-right");
                } else {
                    $(sidebar.Element).removeClass("min-sidebar");
                    $(this).removeClass("icon-double-angle-right").addClass("icon-double-angle-left");
                }
                return false;
            });
            $("<div></div>").addClass("frame-sidebar-collapse").append(sidebar.Collapse).appendTo(sidebar.Element);
        },
        _SetShortcuts: function (shortcuts) {
            var sidebar = this, options = this.Options;

            if (shortcuts && shortcuts.length > 0) {
                sidebar.ShortcutsContainer = $("<div></div>").addClass("frame-sidebar-shortcuts").appendTo(sidebar.Element);
                sidebar._CreateLargeShortcuts(shortcuts);
                sidebar._CreateMiniShortcuts(shortcuts);
            }
        },
        _CreateLargeShortcuts: function (shortcuts) {
            var sidebar = this, options = this.Options;
            var container = $("<div></div>").addClass("frame-sidebar-shortcuts-large").appendTo(sidebar.ShortcutsContainer);
            $(shortcuts).each(function () {
                var button = $("<button></button>").addClass("frame-button").on("click", this.OnClick);
                var icon = $("<i></i>").addClass(this.Icon).appendTo(button);
                this.Mode && button.addClass("frame-button-" + this.Mode);
                this.Title && $(button).Tip({ Placement: "auto", Mode: "inverse", Title: this.Title });
                button.appendTo(container);
            });
        },
        _CreateMiniShortcuts: function (shortcuts) {
            var sidebar = this, options = this.Options;
            var container = $("<div></div>").addClass("frame-sidebar-shortcuts-mini").appendTo(sidebar.ShortcutsContainer);
            $(shortcuts).each(function () {
                var button = $("<span></span>").addClass("frame-button");
                this.Mode && button.addClass("frame-button-" + this.Mode);
                button.appendTo(container);
            });
        },
        _SetContent: function (content) {
            var sidebar = this, options = this.Options;

            if (content) {
                var detail = ("function" == typeof content ? content.call(sidebar.Element) : content);
                $(sidebar.Element).append($(content));
            }
        }
    });

})(jQuery);

/**
* jQuery FrameUI 2.1.0
* 
* 作者：zlx
* 日期：2015.06.22
* 说明：Spinner 控件。
*/
(function ($) {
    $.fn.Spinner = function (options) {
        return $.Frame.Do.call(this, "Spinner", arguments);
    };

    $.Methods.Spinner = $.Methods.Spinner || {};

    $.Defaults.Spinner = {
        Type: "float",     // int,float,time
        Value: null,
        Width: 100,
        ReadOnly: null,
        Disabled: null,
        Step: 0.1,         // 每次递增或递减的步长值
        IsNegative: false, // 是否只能为负数
        Place: 2,          // 小数保留位数,当Type=float时生效
        MinValue: 0,
        MaxValue: 100,
        OnChange: null
    };

    $.Frame.Controls.Spinner = function (element, options) {
        $.Frame.Controls.Spinner.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Spinner.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Spinner";
        },
        _IdPrev: function () {
            return "SpinnerFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Spinner;
        },
        _PreRender: function () {
            var spinner = this, options = this.Options;

            if (!$(spinner.Element).hasClass("frame-spinner")) {
                $(spinner.Element).addClass("frame-spinner");
            }

            spinner.Disabled = false;
            spinner.oldValue = null;
        },
        _Render: function () {
            var spinner = this, options = this.Options;

            spinner.Container = $("<div></div>").addClass("frame-spinner-container");
            spinner.Group = $("<div></div>").addClass("frame-spinner-group");
            $(spinner.Element).wrap(spinner.Group);
            $(spinner.Element).parent().wrap(spinner.Container);

            spinner.DeButtonGroup = $("<div></div>").addClass("frame-spinner-buttons").insertBefore(spinner.Element)
                                                    .on("click", function () { spinner._DoDecrease(); });
            spinner.DeButton = $("<button></button>").addClass("frame-button").addClass("frame-button-error")
                                                     .append($("<i></i>").addClass("icon-minus").addClass("smaller-75"))
                                                     .appendTo(spinner.DeButtonGroup);

            spinner.InButtonGroup = $("<div></div>").addClass("frame-spinner-buttons").insertAfter(spinner.Element)
                                                    .on("click", function () { spinner._DoIncrease(); });
            spinner.InButton = $("<button></button>").addClass("frame-button").addClass("frame-button-success")
                                                     .append($("<i></i>").addClass("icon-plus").addClass("smaller-75"))
                                                     .appendTo(spinner.InButtonGroup);

            $(spinner.Element).on("change", function () {
                var value = $(this).val();
                var newValue = spinner._GetVerifyValue(value);
                if (spinner.HasBind("Change") && spinner.oldValue != newValue)
                    spinner.Trigger("Change", [newValue]);
                $(this).val(newValue);
                return false;
            });
            spinner.Set(options);
        },
        _SetWidth: function (width) {
            var spinner = this, options = this.Options;
            var btnWidth = spinner.DeButtonGroup.outerWidth(true);

            $(spinner.Element).parent().parent().width(width);
            $(spinner.Element).width(width - btnWidth * 2 - 10);
        },
        _SetValue: function (value) {
            var spinner = this, options = this.Options;
            if (value) {
                value = spinner._GetVerifyValue(value);
                $(spinner.Element).val(value);
                $(spinner.Element).trigger("change");
            } else {
                $(spinner.Element).val(spinner._GetDefaultValue());
            }
        },
        _GetValue: function () {
            var spinner = this;
            return $(spinner.Element).val();
        },
        _SetDisabled: function (disabled) {
            var spinner = this;
            if (disabled) {
                $(spinner.Element).attr("disabled", "disabled");
                spinner.DeButton.attr("disabled", "disabled");
                spinner.DeButton.addClass("disabled");
                spinner.InButton.attr("disabled", "disabled");
                spinner.InButton.addClass("disabled");
                spinner.Disabled = true;
            } else {
                $(spinner.Element).removeAttr("disabled");
                spinner.DeButton.removeAttr("disabled");
                spinner.DeButton.removeClass("disabled");
                spinner.InButton.removeAttr("disabled");
                spinner.InButton.removeClass("disabled");
                spinner.Disabled = false;
            }
        },
        _GetDisabled: function () {
            var spinner = this;
            return spinner.Disabled;
        },
        _SetReadOnly: function (readonly) {
            var spinner = this;
            if (readonly != null) {
                if (readonly)
                    $(spinner.Element).attr("readonly", "readonly");
                else
                    $(spinner.Element).removeAttr("readonly");
            }
        },
        _GetReadOnly: function () {
            var spinner = this;
            return $(spinner.Element).attr("readonly");
        },
        _IsInt: function (value) {
            ///<summary>
            /// 判断数值是否为整数
            ///</summary>
            var spinner = this, options = this.Options;
            var rule = options.IsNegative ? /^-\d+$/ : /^\d+$/;

            if (!rule.test(value))
                return false;
            if (parseInt(value) != value)
                return false;

            return true;
        },
        _IsFloat: function (value) {
            ///<summary>
            /// 判断数值是否为浮点数
            ///</summary>
            var spinner = this, options = this.Options;
            var rule = options.IsNegative ? /^-\d+(\.\d+)?$/ : /^\d+(\.\d+)?$/;

            if (!rule.test(value))
                return false;
            if (parseFloat(value) != value)
                return false;

            return true;
        },
        _IsTime: function (value) {
            ///<summary>
            /// 判断是否为时间
            ///</summary>
            var spinner = this, options = this.Options;
            var time = value.match(/^(\d{1,2}):(\d{1,2})$/);

            if (time == null)
                return false;
            if (time[1] >= 24 || time[2] >= 60)
                return false;
            return true;
        },
        _GetVerifyValue: function (value) {
            ///<summary>
            /// 获取经过格式验证处理后的数值
            ///</summary>
            ///<param name="value" type="number">要进行格式验证处理的数值</param>
            var spinner = this, options = this.Options;
            var newValue = null;
            switch (options.Type) {
                case "float":
                    newValue = spinner._ToRound(value, options.Place);
                    break;
                case "time":
                    newValue = value;
                    break;
                default:
                    newValue = parseInt(value);
                    break;
            }
            if (!spinner._Verify(newValue))
                return spinner._GetDefaultValue();
            else
                return newValue;
        },
        _Verify: function (value) {
            ///<summary>
            /// 验证数值是否符合指定类型格式且不超过数值范围
            ///</summary>
            ///<param name="value" type="number">验证的数值</param>
            var spinner = this, options = this.Options;
            var result = true;
            var val;
            switch (options.Type) {
                case "float":
                    if (!spinner._IsFloat(value))
                        result = false;
                    else {
                        val = parseFloat(value);
                        result = spinner._VerifyNumberRange(val);
                    }
                    break;
                case "time":
                    result = spinner._IsTime(value);
                    break;
                default:
                    if (!spinner._IsInt(value))
                        result = false;
                    else {
                        val = parseInt(value);
                        result = spinner._VerifyNumberRange(val);
                    }
                    break;
            }

            return result;
        },
        _VerifyNumberRange: function (value) {
            ///<summary>
            /// 验证数值是否超过限制范围
            ///</summary>
            ///<param name="value" type="number">验证的数值</param>
            var spinner = this, options = this.Options;

            if (options.MinValue != undefined && options.MinValue > value)
                return false;
            if (options.MaxValue != undefined && options.MaxValue < value)
                return false;
            return true;
        },
        _GetDefaultValue: function () {
            ///<summary>
            /// 根据配置类型返回文本框显示的默认值
            ///</summary>
            var spinner = this, options = this.Options;
            var val = null;
            switch (options.Type) {
                case "time":
                    val = "00:00";
                    break;
                case "float":
                    val = spinner._ToRound(0, options.Place);
                    break;
                default:
                    val = 0;
                    break;
            }

            return val;
        },
        _ToRound: function (value, place) {
            ///<summary>
            /// 将指定数值转换为保留指定位数
            ///</summary>
            ///<param name="value" type="number">要保留指定小数位的数值</param>
            ///<param name="place" type="number">数值要保留的小数位位数</param>
            var spinner = this, options = this.Options;
            return parseFloat(value).toFixed(place);
        },
        _CalcValue: function (step) {
            var spinner = this, options = this.Options;
            var value = $(spinner.Element).val();
            spinner.Set({ Value: parseFloat(value) + step });
        },
        _CalcTime: function (step) {
            var spinner = this, options = this.Options;
            var value = $(spinner.Element).val();
            var matchValue = value.match(/^(\d{1,2}):(\d{1,2})$/);
            var newMinute = parseInt(matchValue[2]) + step;
            var newHour = parseInt(matchValue[1]);

            if (newMinute < 10 && newMinute >= 0)
                newMinute = "0" + newMinute;
            if (newMinute == 60) {
                newHour += step;
                newMinute = "00";
            }
            if (newHour == 24) {
                newHour = "00";
            }
            if (newMinute < 0) {
                newHour += step;
                newMinute = 59;
            }
            if (newHour < 10)
                newHour = "0" + newHour;

            value = newHour + ":" + newMinute;
            spinner.Set({ Value: value });
        },
        _DoIncrease: function () {
            ///<summary>
            /// 向上递增
            ///</summary>
            var spinner = this, options = this.Options;

            if (options.Disabled || options.ReadOnly)
                return;

            switch (options.Type) {
                case "time":
                    spinner._CalcTime(options.Step);
                    break;
                default:
                    spinner._CalcValue(options.Step);
                    break;
            }
        },
        _DoDecrease: function () {
            ///<summary>
            /// 向下递减
            ///</summary>
            var spinner = this, options = this.Options;

            if (spinner.Disabled || options.ReadOnly)
                return;

            switch (options.Type) {
                case "time":
                    spinner._CalcTime(options.Step * -1);
                    break;
                default:
                    spinner._CalcValue(options.Step * -1);
                    break;
            }
        },
        Enable: function () {
            var spinner = this, options = this.Options;
            spinner.Set({ Disabled: false });
        },
        Disable: function () {
            var spinner = this, options = this.Options;
            spinner.Set({ Disabled: true });
        },
        SetValue: function (value) {
            var spinner = this;
            spinner.Set({ Value: value });
        },
        GetValue: function () {
            var spinner = this;
            return $(spinner.Element).val();
        }
    });

})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.06.06
* 说明：Tabs 标签页控件。
*/
(function ($) {

    $.fn.Tabs = function (options) {
        return $.Frame.Do.call(this, "Tabs", arguments);
    };

    $.Methods.Tabs = $.Methods.Tabs || {};

    $.Defaults.Tabs = {
        Tabs: [],          // 选项卡对象格式：{Title,Url,Content,Closable,Selected,Disabled,Icon:{color,cls}}
        Width: "auto",
        Height: "auto",
        Plain: false,       // 是否为简洁模式,即无边框
        OnLoad: null,
        OnSelect: null,
        OnBeforeClose: null,
        OnClose: null,
        OnAdd: null
    };

    $.Frame.Controls.Tabs = function (element, options) {
        $.Frame.Controls.Tabs.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Tabs.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Tabs";
        },
        _IdPrev: function () {
            return "TabsFor";
        },
        _ExtenMethods: function () {
            return $.Methods.Tabs;
        },
        _PreRender: function () {
            var tabs = this;
            if (!$(tabs.Element).hasClass("frame-tabs")) {
                $(tabs.Element).addClass("frame-tabs");
            }
            tabs.Children = [];
            tabs._ResetTabItemsExistsInElement();
        },
        _Render: function () {
            var tabs = this, options = this.Options;
            tabs.NavHeader = $("<div></div>").addClass("frame-tabs-nav-wrap");
            tabs.PrevButton = $("<i></i>").addClass("icon-angle-left").on("click", function () { tabs._Prev(); }).hide();
            tabs.NextButton = $("<i></i>").addClass("icon-angle-right").on("click", function () { tabs._Next(); }).hide();
            tabs.NavContainer = $("<div></div>").addClass("frame-tabs-nav-container");
            tabs.NavBar = $("<ul></ul>").addClass("frame-tabs-nav").appendTo(tabs.NavContainer);

            $(tabs.Element).append(tabs.NavHeader);
            $(tabs.NavHeader).append(tabs.PrevButton).append(tabs.NavContainer).append(tabs.NextButton);

            tabs.Contents = $("<div></div>").addClass("frame-tabs-content");
            if (options.Plain) {
                tabs.Contents.addClass("plain");
            }
            $(tabs.Element).append(tabs.Contents);

            tabs.Set(options);

        },
        _ResetTabItemsExistsInElement: function () {
            ///<summary>
            /// 将页面元素已存在的子元素追加到Options配置对象的Tabs属性中,以便在构造Tabs控件时使用
            ///</summary>
            var tabs = this, options = this.Options;

            if ($(tabs.Element).children().length > 0) {
                $(tabs.Element).children().each(function () {
                    var option = {
                        Title: $(this).attr("title"),
                        Url: null,
                        Content: $(this),
                        Disabled: $(this).attr("disabled"),
                        Selected: $(this).attr("selected"),
                        Closable: $(this).attr("closable"),
                        Icon: $(this).attr("icon") ? $.extend({}, (new Function("return {" + $(this).attr("icon") + "}"))()) : false
                    };

                    $(this).removeAttr("title").removeAttr("url").removeAttr("disabled")
                           .removeAttr("selected").removeAttr("closable").removeAttr("icon");
                    // 向Options配置的Tabs数组属性的开头添加包装好的配置项
                    tabs.Children.unshift(option);
                    $(this).remove();
                });
            }
        },
        _SetTabs: function (items) {
            ///<summary>
            /// 设置选项卡
            ///</summary>
            ///<param name="items" type="array">标签页对象数组集合.对象结构:[Title,Url,Content,Closable,Selected,Disable,Icon{cls,color}]</param>
            var tabs = this, options = this.Options;

            tabs.NavBarWidth = 0;
            $.merge(tabs.Children, items);
            $(tabs.Children).each(function (idx) {
                tabs._CreateTabItem(idx, this);
            });

            // 默认选中第一个选项卡
            var sender = tabs.NavBar.children().eq(0);
            if (sender.length > 0) {
                tabs._ToggleTab(sender);
            }
        },
        _CreateTabItemButton: function (idx, id, title, icon, closable) {
            var tabs = this;
            var item = $("<a></a>").attr("href", "#" + id).attr("idx", idx);
            if (icon) {
                var i = $("<i></i>").addClass(icon.color).addClass(icon.cls).addClass("bigger-110");
                item.append(i);
            }
            item.append(title);
            if (closable) {
                var btn = $("<i></i>").addClass("with-x").text("×")
                                .on("click", function () {
                                    if ($(this).parent().parent().hasClass("disabled"))
                                        return;
                                    var selected = $(this).parent().parent().hasClass("active");
                                    var idx = parseInt($(this).parent().attr("idx"));
                                    tabs.Close(idx);
                                    idx -= 1;
                                    if (idx >= 0 && selected) {
                                        tabs.SelectTab(idx);
                                    }
                                });
                item.append(btn);
            }
            return item;
        },
        _CreateTabItemContent: function (idx, id, url, content) {
            var tabContent = $("<div></div>").attr("id", id).attr("idx", idx).addClass("tab-panel");
            if (url) {
                var framename = "Frame" + new Date().getTime();
                var iframe = $('<iframe></iframe>').attr("id", framename)
                .attr("name", framename).attr("src", url).attr("frameborder", "0").attr("scrolling", "no");

                tabContent.append(iframe);
            } else {
                tabContent.append(content);
            }

            return tabContent;
        },
        _CreateTabItemId: function (idx) {
            var tabs = this;
            return tabs.Id + "_ti_" + idx;
        },
        _SetWidth: function (width) {
            var tabs = this, options = this.Options;

            if (width != "auto") {
                $(tabs.Element).width(width);
            }

            tabs._ToggleButton();
        },
        _SetHeight: function (height) {
            var tabs = this, options = this.Options;
            if (height != "auto") {
                var wrapHeight = tabs.NavHeader.height();
                var diff = options.Plain ? 0 : 2;
                $(tabs.Element).height(height);
                tabs.Contents.height(height - wrapHeight - diff);
            } else {
                tabs.Contents.addClass("no-overflow");
            }
        },
        _ToggleButton: function () {
            var tabs = this;
            var wrapWidth = tabs.NavHeader.width();
            if (tabs.NavBarWidth > wrapWidth) {
                tabs.PrevButton.show();
                tabs.NextButton.show();
                tabs.NavContainer.width(wrapWidth - 26);
                tabs.NavBar.children().first().css("margin-left", "0px");
            } else {
                tabs.PrevButton.hide();
                tabs.NextButton.hide();
                tabs.NavContainer.width(wrapWidth + 26);
                tabs.NavBar.children().first().css("margin-left", "5px");
                tabs.NavBar.animate({ left: 0 });
            }
        },
        _GetPrevMoveList: function () {
            ///<summary>
            /// 记录每个项的left,由左到右
            ///</summary>
            var tabs = this;
            var list = new Array();
            tabs.NavBar.children().each(function (i) {
                var current = 0;
                if (i > 0) {
                    current = parseInt(list[i - 1]) + $(this).prev().width();
                }
                list.push(current);
            });

            return list;
        },
        _GetNextMoveList: function () {
            ///<summary>
            /// 记录每个项的left,由右到左
            ///</summary>
            var tabs = this;
            var list = new Array();
            var barWidth = tabs.NavContainer.width();

            var itemLength = tabs.NavBar.children().length;
            for (var i = itemLength - 1; i >= 0; i--) {
                var current = tabs.NavBarWidth - barWidth + 2;
                if (i != itemLength - 1) {
                    current = parseInt(list[itemLength - 2 - i]) - tabs.NavBar.children().eq(i + 1).width();
                }
                list.push(current);
            }
            return list;
        },
        _Prev: function () {
            var tabs = this;
            var leftList = tabs._GetPrevMoveList();
            var currentLeft = -1 * parseInt(tabs.NavBar.css("left"));
            for (var i = 0; i < leftList.length - 1; i++) {
                if (leftList[i] < currentLeft && leftList[i + 1] >= currentLeft) {
                    tabs.NavBar.animate({ left: -1 * parseInt(leftList[i]) });
                    return;
                }
            }
        },
        _Next: function () {
            var tabs = this;
            var rightList = tabs._GetNextMoveList();

            var currentRight = -1 * parseInt(tabs.NavBar.css("left"));
            for (var i = 1; i < rightList.length; i++) {
                if (rightList[i] <= currentRight && rightList[i - 1] > currentRight) {
                    tabs.NavBar.animate({ left: -1 * parseInt(rightList[i - 1]) });
                    return;
                }
            }
        },
        SelectTab: function (which) {
            ///<summary>
            /// 选择指定的选项卡
            ///</summary>
            ///<param name="which" type="which">选项卡的索引值或名称</param>
            var tabs = this, options = this.Options;

            if (!tabs.Exists(which))
                return;

            var index = (typeof which == "number") ? which : tabs.GetTabIndex(which);
            var sender = tabs.NavBar.children().eq(index);

            if (tabs.HasBind("Select"))
                tabs.Trigger("Select", [sender, index]);

            if (tabs.Children[index].Selected)
                return tabs.Children[index];

            if (sender.length > 0) {
                if (tabs.HasBind("Load") && tabs.Children[index].Url)
                    tabs.Trigger("Load", [sender, $("iframe", tabs.Contents.children().eq(index))]);
                tabs._ToggleTab(sender);
                return tabs.Children[index];
            }
            else
                return;
        },
        Add: function (tab) {
            ///<summary>
            /// 添加一个选项卡
            ///</summary>
            ///<param name="tab" type="tab">选项卡对象</param>
            var tabs = this, options = this.Options;

            if (tabs.Exists(tab.Title))
                return;

            //            this.Options.Tabs.push(tab);
            //            var idx = this.Options.Tabs.length - 1;
            tabs.Children.push(tab);
            var idx = tabs.Children.length - 1;

            tabs._CreateTabItem(idx, tab);
            tabs._ToggleButton();
            tabs.SelectTab(idx);

            if (tabs.HasBind("Add"))
                tabs.Trigger("Add", [tabs.NavBar.children().eq(idx), idx]);
        },
        AddRange: function (items) {
            ///<summary>
            /// 添加多个选项卡
            ///</summary>
            ///<param name="items" type="array">选项卡对象集合</param>
            var tabs = this, options = this.Options;

            $(items).each(function () {
                if (!tabs.Exists(this.Title)) {
                    tabs.Children.push(this);
                    var idx = tabs.Children.length - 1;
                    tabs._CreateTabItem(idx, this);
                }
            });

            tabs._ToggleButton();
            tabs.SelectTab(tabs.Children.length - 1);
        },
        _CreateTabItem: function (idx, data) {
            var tabs = this, options = this.Options;
            var id = tabs._CreateTabItemId(idx);

            var tab = $("<li></li>").attr("idx", idx).on("click", function (e) {
                tabs._ToggleTab(this);
                return false;
            });

            var item = tabs._CreateTabItemButton(idx, id, data.Title, data.Icon, data.Closable);
            if (data.Selected) {
                tab.addClass("active");
            } else
                data.Selected = false;
            if (data.Disabled) {
                tab.addClass("disabled");
            }
            else
                data.Disabled = false;
            tab.append(item);
            tabs.NavBar.append(tab);
            tabs.NavBarWidth += tab.outerWidth(true); // 这里合计标签页头的实际宽度
            tabs.Contents.append(tabs._CreateTabItemContent(idx, id, data.Url, $(data.Content)));
        },
        _ToggleTab: function (tab) {
            var tabs = this, options = this.Options;

            var activeTab = $("li.active", tabs.NavBar);
            if (activeTab.length > 0) {
                activeTab.removeClass("active");
                tabs.Children[activeTab.attr("idx")].Selected = false;
            }
            $(tab).addClass("active");
            tabs.Children[$(tab).attr("idx")].Selected = true;

            $(tabs.Contents).children(".active").removeClass("active");
            var panel = $($("a", tab).attr("href"));
            panel.addClass("active");

            var iframe = $($("a", tab).attr("href")).children("iframe");
            if (iframe.length > 0) {
                var doc = iframe[0].contentDocument;
                if (doc != null) {
                    panel.height(doc.body.scrollHeight);
                }
            }
        },
        Exists: function (which) {
            ///<summary>
            /// 返回一个值，该值标识是否已存在指定索引或名称的选项卡
            ///</summary>
            ///<param name="which" type="which">选项卡的索引值或名称</param>
            var tabs = this, options = this.Options;
            var exists = false;

            if (tabs.Children.length == 0)
                return false;

            if (typeof which == "number") {
                if (which < 0 || tabs.Children.length <= which)
                    return false;
                return tabs.Children[which];
            }
            if (typeof which == "string") {
                $(tabs.Children).each(function () {
                    if (this.Title == which) {
                        exists = true;
                        return;
                    }
                });

                return exists;
            }

            return false;
        },
        GetTabIndex: function (title) {
            ///<summary>
            /// 获取指定名称的选项卡的对应索引值
            ///</summary>
            ///<param name="title" type="string">选项卡的名称</param>
            var tabs = this, options = this.Options;

            var index = -1;
            $(tabs.Children).each(function (idx) {
                if (this.Title == title) {
                    index = idx;
                    return;
                }
            });

            return index;
        },
        GetSelectedTab: function () {
            ///<summary>
            /// 获取当前被选中的选项卡
            ///</summary>
            var tabs = this, options = this.Options;
            var active = $("li.active", tabs.NavBar);
            if (active.length > 0) {
                var idx = active.attr("idx");
                return tabs.Children[idx];
            }
            else
                return;
        },
        Close: function (which) {
            ///<summary>
            /// 关闭一个选项卡面板
            ///</summary>
            ///<param name="which" type="which">表示哪个选项卡将被关闭,参数指向选项卡标题或索引。</param>
            var tabs = this, options = this.Options;

            if (!tabs.Exists(which))
                return;

            var index = (typeof which == "number") ? which : tabs.GetTabIndex(which);
            var isClose = true;
            var tab = tabs.NavBar.children().eq(index);

            if (tabs.HasBind("BeforeClose"))
                isClose = tabs.Trigger("BeforeClose", [tab, index]);
            if (!isClose)
                return;

            tabs.NavBarWidth -= tab.outerWidth(true);
            tab.remove();
            tabs.Contents.children().eq(index).remove();
            tabs.Children.splice(index, 1);
            tabs._RefreshIndex();
            tabs._ToggleButton();

            if (tabs.HasBind("Close"))
                tabs.Trigger("Close", []);
        },
        _RefreshIndex: function () {
            var tabs = this, options = this.Options;
            $(tabs.Children).each(function (idx) {
                var navItem = tabs.NavBar.children().eq(idx);
                var id = tabs._CreateTabItemId(idx);
                navItem.attr("idx", idx);
                $("a", navItem).attr("idx", idx).attr("href", "#" + id);
                tabs.Contents.children().eq(idx).attr("idx", idx).attr("id", id);
            });
        }
    });
})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.01
* 说明：Tags 标签控件。
*/
(function ($) {
    $.fn.Tags = function (options) {
        return $.Frame.Do.call(this, "Tags", arguments);
    };

    $.Methods.Tags = $.Methods.Tags || {};

    $.Defaults.Tags = {
        Items: [],    // {Text,Closable}
        Plain: true,  // 是否为简洁模式,即无边框
        Width: null,
        OnItemClick: null
    };

    $.Frame.Controls.Tags = function (element, options) {
        $.Frame.Controls.Tags.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Tags.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Tags";
        },
        _IdPrev: function () {
            return "TagsFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Tags;
        },
        _PreRender: function () {
            var tags = this;

            if (!$(tags.Element).hasClass("frame-tags")) {
                $(tags.Element).addClass("frame-tags");
            }

            tags.Children = [];
        },
        _Render: function () {
            var tags = this, options = this.Options;

            $(tags.Element).empty();
            tags.Set(options);
        },
        _SetPlain: function (plain) {
            var tags = this;
            if (!plain)
                $(tags.Element).addClass("frame-tags-bordered");
        },
        _SetWidth: function (width) {
            var tags = this;
            if (width)
                $(tags.Element).width(width);
        },
        _SetItems: function (items) {
            var tags = this, options = this.Options;
            if (items && items.length > 0) {
                $(items).each(function (i) {
                    var item = this;
                    tags.Children.push(item);
                    var tag = $("<li></li>").text(item.Text).on("click", function () {
                        $(".frame-tags-selected", tags.Element).removeClass("frame-tags-selected");
                        $(this).addClass("frame-tags-selected");
                        if (tags.HasBind("ItemClick"))
                            tags.Trigger("ItemClick", [this, $(tags.Element).children().index($(this)[0])]);
                    });
                    tag.appendTo(tags.Element);
                    if (item.Closable) {
                        tag.addClass("with-x");
                        $("<i>×</i>").appendTo(tag).on("click", function () {
                            var idx = $(tags.Element).children().index($(this).parent()[0]);
                            tags.RemoveTag(idx);
                        });
                    }
                });
            }
        },
        RemoveTag: function (which) {
            var tags = this, options = this.Options;

            if (!tags.Exists(which))
                return;

            var index = (typeof which == "number") ? which : tags.GetTagIndex(which);
            $(tags.Element).children().eq(index).remove();
            tags.Children.splice(index, 1);
            options.Items.splice(index, 1);
        },
        AppendTag: function (item) {
            var tags = this, options = this.Options;

            if (tags.Exists(item.Text))
                return;

            options.Items.push(item);
            tags._SetItems([item]);
        },
        AppendRangeTag: function (items) {
            var tags = this, options = this.Options;
            $(items).each(function () {
                tags.AppendTag(this);
            });
        },
        Exists: function (which) {
            ///<summary>
            /// 返回一个值，该值标识是否已存在指定索引或名称的标签
            ///</summary>
            ///<param name="which" type="which">标签的索引值或名称</param>
            var tags = this, options = this.Options;
            var exists = false;

            if (tags.Children.length == 0)
                return false;

            if (typeof which == "number") {
                if (which < 0 || tags.Children.length <= which)
                    return false;
                return true;
            }
            if (typeof which == "string") {
                $(tags.Children).each(function () {
                    if (this.Text == which) {
                        exists = true;
                        return;
                    }
                });

                return exists;
            }

            return false;
        },
        GetTagIndex: function (text) {
            ///<summary>
            /// 获取指定名称的标签的对应索引值
            ///</summary>
            ///<param name="text" type="string">标签的名称</param>
            var tags = this, options = this.Options;

            var index = -1;
            $(tags.Children).each(function (idx) {
                if (this.Text == text) {
                    index = idx;
                    return;
                }
            });

            return index;
        }
    });

})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.06.13
* 说明：TextBox 文本框控件。
*/
(function ($) {
    $.fn.TextBox = function (options) {
        return $.Frame.Do.call(this, "TextBox", arguments);
    };

    $.Methods.TextBox = $.Methods.TextBox || {};

    $.Defaults.TextBox = {
        Icon: null,
        Size: "medium",   // mini,small,medium,large,xlarge,xxlarge
        IconSite: "left", // left,right
        IconColor: "blue",
        Value: null,
        ReadOnly: false,
        Disabled: false,
        CanMultiLine: false,
        AutoSize: false,
        Width: null,
        Height: null,
        OnBeforeFocus: null,
        OnFocus: null,
        OnBlur: null,
        OnChange: null
    };

    $.Frame.Controls.TextBox = function (element, options) {
        $.Frame.Controls.TextBox.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.TextBox.Extension($.Frame.Controls.Base, {
        _GetType: function () {
            return "TextBox";
        },
        _IdPrev: function () {
            return "TextBoxFor";
        },
        _ExtendMethods: function () {
            return $.Methods.TextBox;
        },
        _PreRender: function () {
            var textbox = this;

            if (!$(textbox.Element).hasClass("frame-textbox")) {
                $(textbox.Element).addClass("frame-textbox");
            }
        },
        _Render: function () {
            var textbox = this, options = this.Options;

            $(textbox.Element).on("focus.TextBox", function () {
                var ifcontinue = true;
                if (textbox.HasBind("BeforeFocus"))
                    ifcontinue = textbox.Trigger("BeforeFocus", [this]);
                if (ifcontinue) {
                    if (textbox.HasBind("Focus"))
                        textbox.Trigger("Focus", [this]);
                }
                return false;
            }).on("blur.TextBox", function () {
                if (textbox.HasBind("Blur"))
                    textbox.Trigger("Blur", [this]);
                return false;
            }).on("keyup.TextBox", function () {
                if (textbox.HasBind("Change"))
                    textbox.Trigger("Change", [this.value]);
                return false;
            });

            textbox.Set(options);
        },
        _SetIcon: function (icon) {
            var textbox = this, options = this.Options;

            // TODO:文本框不能是多行文本框，该属性才能生效
            if (icon && !options.CanMultiLine) {
                textbox.Wrapper = $("<div></div>").addClass("input-icon");
                if (options.IconSite == "right")
                    textbox.Wrapper.addClass("input-icon-right");

                $(textbox.Element).wrap(textbox.Wrapper);
                $("<i></i>").addClass(icon).addClass(options.IconColor).insertBefore(textbox.Element);

            }
        },
        _SetSize: function (size) {
            ///<summary>
            /// 设置文本框的尺寸样式
            ///</summary>
            ///<param name="size" type="string">尺寸类型.[mini,small,medium,large,xlarge,xxlarge]</param>
            var textbox = this, options = this.Options;
            if (size) {
                if (textbox.Wrapper) {
                    $(textbox.Element).parent().addClass("input-" + size);
                    $(textbox.Element).addClass("input-" + size);
                }
                else
                    $(textbox.Element).addClass("input-" + size);
            }
        },
        _SetValue: function (value) {
            var textbox = this, options = this.Options;
            if (value) {
                $(textbox.Element).val(value);
            }
        },
        _GetValue: function () {
            var textbox = this, options = this.Options;
            return $(textbox.Element).val();
        },
        _SetReadOnly: function (readonly) {
            var textbox = this, options = this.Options;
            if (readonly != null) {
                if (readonly)
                    $(textbox.Element).attr("readonly", "readonly");
                else
                    $(textbox.Element).removeAttr("readonly");
            }
        },
        _GetReadOnly: function () {
            var textbox = this, options = this.Options;
            return $(textbox.Element).attr("readonly");
        },
        _SetDisabled: function (disabled) {
            var textbox = this, options = this.Options;
            if (disabled != null) {
                if (disabled)
                    $(textbox.Element).attr("disabled", "disabled");
                else
                    $(textbox.Element).removeAttr("disabled");
            }
        },
        _GetDisabled: function () {
            var textbox = this, options = this.Options;
            return $(textbox.Element).attr("disabled");
        },
        _SetWidth: function (width) {
            var textbox = this, options = this.Options;
            if (width) {
                if (textbox.Wrapper){
                    $(textbox.Element).parent().width(width);
                }
                $(textbox.Element).width(width);
            }
        },
        _SetHeight: function (height) {
            ///<summary>
            /// 设置文本框高度
            ///</summary>
            ///<param name="height" type="number">文本框的高度。这里只有当文本框为多行文本框时才能设置生效。</param>
            var textbox = this, options = this.Options;
            if (height && options.CanMultiLine)
                $(textbox.Element).height(height);
        },
        _SetAutoSize: function (auto) {
            var textbox = this, options = this.Options;
            if (auto && options.CanMultiLine) {
                $(textbox.Element).autosize();
            }
        },
        Focus: function () {
            ///<summary>
            /// 设置文本框焦点
            ///</summary>
            var textbox = this, options = this.Options;
            $(textbox.Element).trigger("focus");

            var pos = textbox.Element.value.length;
            var textRange = textbox.Element.createTextRange();
            textRange.collapse(true);
            textRange.moveEnd("character", pos);
            textRange.moveStart("character", pos);
            textRange.select();
        },
        Readable: function () {
            ///<summary>
            /// 设置文本框可读写
            ///</summary>
            var textbox = this, options = this.Options;
            textbox.Set("ReadOnly", false);
        },
        UnReadable: function () {
            ///<summary>
            /// 设置文本框只读
            ///</summary>
            var textbox = this, options = this.Options;
            textbox.Set("ReadOnly", true);
        }
    });

})(jQuery);

/*!
Autosize v1.17.1 - 2013-06-23
Automatically adjust textarea height based on user input.
(c) 2013 Jack Moore - http://www.jacklmoore.com/autosize
license: http://www.opensource.org/licenses/mit-license.php

--用于将TextArea多行文本框设置随文本内容自动拓展高度
*/
(function (e) {
    var t, o = {
        className: "autosizejs",
        append: "",
        callback: !1,
        resizeDelay: 10
    }, i = '<textarea tabindex="-1" style="position:absolute; top:-999px; left:0; right:auto; bottom:auto; border:0; -moz-box-sizing:content-box; -webkit-box-sizing:content-box; box-sizing:content-box; word-wrap:break-word; height:0 !important; min-height:0 !important; overflow:hidden; transition:none; -webkit-transition:none; -moz-transition:none;"/>', n = ["fontFamily", "fontSize", "fontWeight", "fontStyle", "letterSpacing", "textTransform", "wordSpacing", "textIndent"], s = e(i).data("autosize", !0)[0];
    s.style.lineHeight = "99px", "99px" === e(s).css("lineHeight") && n.push("lineHeight"),
    s.style.lineHeight = "", e.fn.autosize = function (i) {
        return i = e.extend({}, o, i || {}), s.parentNode !== document.body && e(document.body).append(s),
        this.each(function () {
            function o() {
                var o, a = {};
                if (t = u, s.className = i.className, l = parseInt(h.css("maxHeight"), 10), e.each(n, function (e, t) {
                    a[t] = h.css(t);
                }), e(s).css(a), "oninput" in u) {
                    var r = u.style.width;
                    u.style.width = "0px", o = u.offsetWidth, u.style.width = r;
                }
            }
            function a() {
                var n, a, r, c;
                t !== u && o(), s.value = u.value + i.append, s.style.overflowY = u.style.overflowY,
                a = parseInt(u.style.height, 10), "getComputedStyle" in window ? (c = window.getComputedStyle(u),
                r = u.getBoundingClientRect().width, e.each(["paddingLeft", "paddingRight", "borderLeftWidth", "borderRightWidth"], function (e, t) {
                    r -= parseInt(c[t], 10);
                }), s.style.width = r + "px") : s.style.width = Math.max(h.width(), 0) + "px", s.scrollTop = 0,
                s.scrollTop = 9e4, n = s.scrollTop, l && n > l ? (u.style.overflowY = "scroll",
                n = l) : (u.style.overflowY = "hidden", d > n && (n = d)), n += p, a !== n && (u.style.height = n + "px",
                w && i.callback.call(u, u));
            }
            function r() {
                clearTimeout(c), c = setTimeout(function () {
                    h.width() !== z && a();
                }, parseInt(i.resizeDelay, 10));
            }
            var l, d, c, u = this, h = e(u), p = 0, w = e.isFunction(i.callback), f = {
                height: u.style.height,
                overflow: u.style.overflow,
                overflowY: u.style.overflowY,
                wordWrap: u.style.wordWrap,
                resize: u.style.resize
            }, z = h.width();
            h.data("autosize") || (h.data("autosize", !0), ("border-box" === h.css("box-sizing") || "border-box" === h.css("-moz-box-sizing") || "border-box" === h.css("-webkit-box-sizing")) && (p = h.outerHeight() - h.height()),
            d = Math.max(parseInt(h.css("minHeight"), 10) - p || 0, h.height()), h.css({
                overflow: "hidden",
                overflowY: "hidden",
                wordWrap: "break-word",
                resize: "none" === h.css("resize") || "vertical" === h.css("resize") ? "none" : "horizontal"
            }), "onpropertychange" in u ? "oninput" in u ? h.on("input.autosize keyup.autosize", a) : h.on("propertychange.autosize", function () {
                "value" === event.propertyName && a();
            }) : h.on("input.autosize", a), i.resizeDelay !== !1 && e(window).on("resize.autosize", r),
            h.on("autosize.resize", a), h.on("autosize.resizeIncludeStyle", function () {
                t = null, a();
            }), h.on("autosize.destroy", function () {
                t = null, clearTimeout(c), e(window).off("resize", r), h.off("autosize").off(".autosize").css(f).removeData("autosize");
            }), a());
        });
    };
})(window.jQuery || window.Zepto);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.06.27
* 说明：Tip 工具提示控件。
*/
(function ($) {
    $.Tip = function (options) {
        return $.Frame.Do.call(options.Target, "Tip", arguments, options);
    };

    $.fn.Tip = function (options) {
        options = $.extend({ IdAttr: "frametipid",
            HasElement: false
        }, $.Defaults.Tip, options || {});

        options.Target = options.Target || $(this);

        this.each(function () {
            if ($(this).attr("frametipid"))
                return;

            if (options.Trigger) {
                switch (options.Trigger) {
                    case "click":
                        $(this).on("click.tip", function () { GetTip().Toggle(); });
                        break;
                    case "focus":
                        $(this).on("focus.tip", function () { GetTip().Show(); });
                        $(this).on("blur.tip", function () { GetTip().Close(); });
                        break;
                    default:
                        $(this).on("mouseenter.tip", function () { GetTip().Show(); });
                        $(this).on("mouseleave.tip", function () { GetTip().Close(); });
                        break;
                }
            }
        });

        function GetTip() {
            if ($(this).attr("frametipid"))
                return $.Frame.Get(this, "frametipid");
            else
                return $.Tip(options);
        };

        //return $.Frame.Get(this, "frametipid");
    };

    $.Methods.Tip = $.Methods.Tip || {};

    $.Defaults.Tip = {
        Mode: null,      //[error,success,warning,info,inverse]
        Html: null,
        Title: null,
        Trigger: "hover", //[hover,focus,click]
        Placement: "auto"
    };

    $.Frame.Controls.Tip = function (options) {
        $.Frame.Controls.Tip.Parent.constructor.call(this, null, options);
    };

    $.Frame.Controls.Tip.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Tip";
        },
        _IdPrev: function () {
            return "TipFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Tip;
        },
        _PreRender: function () {
            var tip = this, options = this.Options;
            options.Target.attr(options.IdAttr, tip.Id);

            if ($(options.Target).attr("tip") != undefined) {
                options.Title = $(options.Target).attr("tip");
            }
        },
        _Render: function () {
            var tip = this, options = this.Options;

            tip.Element = $("<div></div>").addClass("frame-tooltip").addClass("fade").appendTo("body")[0];
            $("<div></div>").addClass("frame-tooltip-arrow").appendTo(tip.Element);
            $("<div></div>").addClass("frame-tooltip-inner").appendTo(tip.Element);

            tip.Set(options);
        },
        _SetMode: function (mode) {
            var tip = this, options = this.Options;
            if (mode) {
                $(tip.Element).addClass("frame-tooltip-" + mode);
            }
        },
        _SetTitle: function (title) {
            var tip = this, options = this.Options;
            var content = ("function" == typeof title ? title.call(tip.Element) : title);

            $(".frame-tooltip-inner", tip.Element)[options.Html ? "html" : "text"](content);
        },
        _IniPlacement: function (placement) {
            var tip = this, options = this.Options;
            var reg = /\s?auto?\s?/i;
            var r = reg.test(placement);
            $(tip.Element).removeClass("fade in top bottom left right");

            // 当placement为auto时，置换为top
            r && (placement = placement.replace(reg, "") || "top");

            $(tip.Element).css("display", "block").addClass(placement);
            var position = tip.GetPosition();
            var w = $(tip.Element).outerWidth(true);
            var h = $(tip.Element).outerHeight(true);

            if (r) {
                var t = placement;
                var ww = $(window).width();
                var wh = $(window).height();
                var stop = document.documentElement.scrollTop || document.body.scrollTop;
                var sleft = document.documentElement.scrollLeft || document.body.scrollLeft;
                if ("bottom" == t && position.top + position.height + h - stop > wh)
                    placement = "top";
                else if ("top" == t && position.top - stop - h < 0)
                    placement = "bottom";
                else if ("right" == t && position.right + w > ww)
                    placement = "left";
                else if ("left" == t && position.left - w < 0)
                    placement = "right";
                else
                    placement = t;
                $(tip.Element).removeClass(t).addClass(placement);
            }
            var offset = tip._GetCalculatedOffset(placement, position, w, h);
            tip._ApplyPlacement(offset, placement);
        },
        _ApplyPlacement: function (offset, placement) {
            var tip = this, options = this.Options;
            var c;
            var w = $(tip.Element).outerWidth(true);
            var h = $(tip.Element).outerHeight(true);
            var margin_top = parseInt($(tip.Element).css("margin-top"), 10);
            var margin_left = parseInt($(tip.Element).css("margin-left"), 10);

            isNaN(margin_top) && (margin_top = 0);
            isNaN(margin_left) && (margin_left = 0);
            offset.top = offset.top + margin_top;
            offset.left = offset.left + margin_left;

            $(tip.Element).offset(offset);
            var nw = $(tip.Element).outerWidth(true);
            var nh = $(tip.Element).outerHeight(true);

            if ("top" == placement && nh != h && (c = !0, offset.top = offset.top + h - nh), /bottom|top/.test(placement)) {
                var k = 0;
                offset.left < 0 && (k = -2 * offset.left, offset.left = 0, $(tip.Element).offset(offset));
            }
            c && $(tip.Element).offset(offset);
        },
        _GetCalculatedOffset: function (placement, position, w, h) {
            switch (placement) {
                case "bottom":
                    return {
                        top: position.top + position.height,
                        left: position.left + position.width / 2 - w / 2
                    };
                case "top":
                    return {
                        top: position.top - h,
                        left: position.left + position.width / 2 - w / 2
                    };
                case "left":
                    return {
                        top: position.top + position.height / 2 - h / 2,
                        left: position.left - w
                    };
                default:
                    return {
                        top: position.top + position.height / 2 - h / 2,
                        left: position.left + position.width
                    };
            }
        },
        Toggle: function () {
            var tip = this, options = this.Options;
            $(tip.Element).hasClass("fade") ? tip.Show() : tip.Close();
        },
        GetPosition: function () {
            var tip = this, options = this.Options;
            var target = options.Target;
            return $.extend({}, "function" == typeof target[0].getBoundingClientRect ? target[0].getBoundingClientRect() : {
                width: target[0].offsetWidth,
                height: target[0].offsetHeight
            }, target.offset());
        },
        Show: function () {
            var tip = this, options = this.Options;
            tip._IniPlacement(options.Placement);
            $(tip.Element).removeClass("fade").addClass("in");
        },
        Close: function () {
            var tip = this, options = this.Options;
            $(tip.Element).removeClass("in").addClass("fade");
            $(tip.Element).remove();
            $.Frame.Remove(tip.Id);
            options.Target.removeAttr(options.IdAttr);
        }
    });

})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.22
* 说明：Wizard 向导控件。
*/
(function ($) {
    $.fn.Wizard = function (options) {
        return $.Frame.Do.call(this, "Wizard", arguments);
    };

    $.Methods.Wizard = $.Methods.Wizard || {};

    $.Defaults.Wizard = {
        Steps: [],       //{Step,StepTarget,Title}
        WizardTarget: null,
        ActionTarget: null,
        OnActive: null,
        OnComplete: null
    };

    $.Frame.Controls.Wizard = function (element, options) {
        $.Frame.Controls.Wizard.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Wizard.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Wizard";
        },
        _IdPrev: function () {
            return "WizardFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Wizard;
        },
        _PreRender: function () {
            var wizard = this;

            if (!$(wizard.Element).hasClass("frame-wizard")) {
                $(wizard.Element).addClass("frame-wizard");
            }
            wizard.Step = -1;
        },
        _Render: function () {
            var wizard = this, options = this.Options;

            wizard.Container = $("<ul></ul>").addClass("frame-wizard-steps").appendTo(wizard.Element);
            $(wizard.Element).attr("wizard-target", options.WizardTarget).attr("action-target", options.ActionTarget);

            wizard.Set(options);
        },
        _SetSteps: function (steps) {
            var wizard = this, options = this.Options;
            if (steps && steps.length > 0) {
                $(steps).each(function () {
                    var step = $("<li></li>").attr("step", this.Step).attr("step-target", this.StepTarget)
                                             .appendTo(wizard.Container);
                    step.append($("<span></span>").addClass("step").text(this.Step));
                    step.append($("<span></span>").addClass("title").text(this.Title));
                    $(this.StepTarget).addClass("frame-wizard-step-pane");
                });
                wizard.Next();
            }
        },
        _SetWizardTarget: function (target) {
            var wizard = this, options = this.Options;
            if (target) {
                $(wizard.Element).attr("wizard-target", target);
                $(target).addClass("frame-wizard-step-content");
            }
        },
        _SetActionTarget: function (target) {
            var wizard = this, options = this.Options;
            if (target) {
                $(wizard.Element).attr("action-target", target);
                $(target).addClass("frame-wizard-actions");
            }
        },
        Previous: function () {
            var wizard = this, options = this.Options;

            if (wizard.Step > 0) {
                wizard.Step--;

                var current = $(wizard.Container).children().eq(wizard.Step);
                var next = current.next();
                if (next && next.length > 0) {
                    next.removeClass("complete").removeClass("active");
                    $(next.attr("step-target")).removeClass("active");
                }
                current.removeClass("complete").addClass("active");
                $(current.attr("step-target")).addClass("active");
            }
        },
        Next: function () {
            var wizard = this, options = this.Options;

            if (wizard.Step < options.Steps.length - 1) {
                wizard.Step++;

                var current = $(wizard.Container).children().eq(wizard.Step);
                var prev = current.prev();
                current.addClass("active");
                $(current.attr("step-target")).addClass("active");
                if (prev && prev.length > 0) {
                    prev.addClass("complete");
                    $(prev.attr("step-target")).removeClass("active");
                    if (wizard.HasBind("Complete"))
                        wizard.Trigger("Complete", [prev, { Step: parseInt(prev.attr("step")), Title: $("span.title", prev).text()}]);
                }
                if (wizard.HasBind("Active"))
                    wizard.Trigger("Active", [current, { Step: parseInt(current.attr("step")), Title: $("span.title", current).text()}]);
            }
        }
    });

})(jQuery);

/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.12.14
* 说明：Validation 表单校验控件。
*/
(function ($) {
    $.fn.Validation = function (options) {
        return $.Frame.Do.call(this, "Validation", arguments);
    };
    $.fn.ValidateInput = function (options) {
        return $.Frame.Do.call(this, "ValidateInput", arguments);
    };

    $.Methods.Validation = $.Methods.Validation || {};
    $.Methods.ValidateInput = $.Methods.ValidateInput || {};

    $.Defaults.Validation = {
        OnError: null,
        OnSuccess: null
    };
    $.Defaults.ValidateInput = {
        OnError: null,
        OnSuccess: null
    };

    $.Frame.Controls.Validation = function (element, options) {
        $.Frame.Controls.Validation.Parent.constructor.call(this, element, options);
    };
    $.Frame.Controls.ValidateInput = function (element, options) {
        $.Frame.Controls.Validation.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Validation.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Validation";
        },
        _IdPrev: function () {
            return "ValidationFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Validation;
        },
        _PreRender: function () {
            var form = this;

            if (!$(form.Element).hasClass("frame-validation")) {
                $(form.Element).addClass("frame-validation");
            }
        },
        _Render: function () {
            var form = this, options = this.Options;

            form.Results = [];
            form.Inputs = [];
            form.Counter = 0;
            form._Init();

            form.Set(options);
        },
        _Init: function () {
            var form = this, options = this.Options;

            form.Results = [];
            form.Inputs.splice(0, form.Inputs.length);
            form.Counter = 0;

            $(form.Element).find("input:visible").each(function (index, input) {
                //排除 hidden、button、submit、checkbox、radio、file
                if (input.type != "hidden" && input.type != "button" && input.type != "submit" && input.type != "checkbox" && input.type != "radio" && input.type != "file") {
                    var checker = $(input).ValidateInput({
                        OnError: function (e) {
                            form.Results.push(e);
                            if (form.HasBind("Error")) {
                               return form.Trigger("Error", [e]);
                            }
                            return true;
                        },
                        OnSuccess: function (e) {
                            form.Counter++;
                            if (form.Counter == form.Inputs.length) {
                                form.Counter = 0;
                                if (form.HasBind("Success")) {
                                  return  form.Trigger("Success", [e]);
                                }
                            }
                            return true;
                        }
                    });

                    form.Inputs.push(checker);
                }
            });
        },
        Validation: function () {
            var form = this, options = this.Options;
            form._Init();

            var index;
            for (index in form.Inputs) {
                form.Inputs[index].Validation();
            }
        }
    });

    $.Frame.Controls.ValidateInput.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "ValidateInput";
        },
        _IdPrev: function () {
            return "ValidateInputFor";
        },
        _ExtendMethods: function () {
            return $.Methods.ValidateInput;
        },
        _PreRender: function () {
            var input = this;

            input.Rules = [];
            input.Counter = 0;
            input.Message = $(input.Element).attr("message");
            input.Message = (!!input.Message ? input.Message : "格式错误!");

        },
        _Render: function () {
            var input = this, options = this.Options;

            input._Init_Popover();
            input.Set(options);

            //是否实时检查
            var ruleString = $(input.Element).attr("rules");
            if (!!ruleString && -1 != ruleString.indexOf("real-time")) {
                $(input.Element).blur(function () {
                    input.Validation();
                });
            }

        },
        _Init_Popover: function () {
            var input = this;

            input.Popover = $(input.Element).Popover({ Mode: "notitle", Placement: "left auto" });
        },
        Validation: function () {
            var input = this;

            input.Value = $(input.Element).val();
            input.Counter = 0;
            input.Rules = [];

            var ruleString = $(input.Element).attr("rules");
            input._Parse(ruleString);

            for (var i = 0; i < input.Rules.length; i++) {
                //调用条件函数
                if (!!input.Judges[input.Rules[i].Rule])
                    input.Judges[input.Rules[i].Rule](input, input.Value, input.Rules[i].Param);
            }
        },
        Judges: {
            "char-number": function (input, value, param) {
                if (value != "" && !(/^[0-9]*$/g.test(value)))
                    return input._Error("char-number");
                else

                    return input._Success("char-number");
            },
            "char-normal": function (input, value, param) {
                if (false == /^\w+$/.test(value))
                    return input._Error("char-normal");
                else
                    return input._Success("char-normal");
            },
            "char-chinese": function (input, value, param) {
                if (false == /^([\w]|[\u4e00-\u9fa5]|[ 。，、？￥“‘！：【】《》（）——+-])+$/.test(value))
                    return input._Error("char-chinese");
                else
                    return input._Success("char-chinese");
            },
            "char-english": function (input, value, param) {
                if (false == /^([\w]|[ .,?!$'":+-])+$/.test(value))
                    return input._Error("char-english");
                else
                    return input._Success("char-english");
            },
            "email": function (input, value, param) {
                if (false == /^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$/.test(value))
                    return input._Error("email");
                else
                    return input._Success("email");
            },
            "length": function (input, value, param) {
                var range = param.split("-");

                //如果长度设置为 length:6 这样的格式
                if (range.length == 1) range[1] = range[0];

                if (value.length < range[0] || value.length > range[1])
                    return input._Error("length");
                else
                    return input._Success("length");
            },
            "equal": function (input, value, param) {
                var pair = $(param);
                if (0 == pair.length || pair.val() != value)
                    return input._Error("equal");
                else
                    return input._Success("equal");
            },
            "ajax": function (input, value, param) {
                if (false == eval(param))
                    return input._Error("ajax");
                else
                    return input._Success("ajax");
            },
            "date": function (input, value, param) {
                if (false == /^(\d{4})-(\d{2})-(\d{2})$/.test(value))
                    return input._Error("date");
                else
                    return input._Success("date");
            },
            "time": function (input, value, param) {
                if (false == /^(\d{2}):(\d{2}):(\d{2})$/.test(value))
                    return input._Error("time");
                else
                    return input._Success("time");
            },
            "datetime": function (input, value, param) {
                if (false == /^(\d{4})-(\d{2})-(\d{2}) (\d{2}):(\d{2}):(\d{2})$/.test(value))
                    return input._Error("datetime");
                else
                    return input._Success("datetime");
            },
            "money": function (input, value, param) {
                if (false == /^([1-9][\d]{0,7}|0)(\.[\d]{1,2})?$/.test(value))
                    return input._Error("money");
                else
                    return input._Success("money");
            },
            "uint": function (input, value, param) {
                value = parseInt(value);
                param = parseInt(param);

                if (isNaN(value) || isNaN(param) || value < param || value < 0)
                    return input._Error("uint");
                else
                    return input._Success("uint");
            },
            "require": function (input, value, param) {
                if (!value) {
                    if (!!param && -1 != param.indexOf("require")) //rule不为空并且含有require
                        return input._Error("require");
                    else
                        return input._Success("require");

                }
            }
        },
        _Parse: function (ruleString) {
            var input = this;

            input.Rules = [];

            var rules = !!ruleString ? ruleString.split(";") : {};

            for (var i = 0; i < rules.length; i++) {
                var s = rules[i];
                var rule = s;
                var param = "";

                //有：号
                var p = s.indexOf(":");
                if (-1 != p) {
                    rule = s.substr(0, p);
                    param = s.substr(p + 1);
                }

                if (!!input.Judges[rule])
                    input.Rules.push({ Rule: rule, Param: param });
            }
        },
        _Error: function (rule) {
            var input = this, options = this.Options;
            var isContinue = true;
            if (input.HasBind("Error")) {
                isContinue = input.Trigger("Error", [rule]);
            }

            if (isContinue) {
                var msg = $(input.Element).attr(rule + "-message");

                var msg = !msg ? this.Message : msg;

                input.Popover.Reset(msg);
                input.Popover.Show();
                return true;
            }

            return false;
        },
        _Success: function (rule) {
            var input = this, options = this.Options;

            input.Counter += 1;

            if (input.Counter == input.Rules.length) {
                var isContinue = true;
                if (input.HasBind("Success")) {
                    isContinue = input.Trigger("Success", [rule]);
                }

                if (isContinue) {
                    input.Popover.Close();
                }
            }

            return true;
        }
    });

})(jQuery);