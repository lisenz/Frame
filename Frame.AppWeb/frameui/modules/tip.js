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