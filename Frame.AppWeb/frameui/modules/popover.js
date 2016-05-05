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