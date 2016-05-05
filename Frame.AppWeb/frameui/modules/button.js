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