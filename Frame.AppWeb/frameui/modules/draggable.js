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