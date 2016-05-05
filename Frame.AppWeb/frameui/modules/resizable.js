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