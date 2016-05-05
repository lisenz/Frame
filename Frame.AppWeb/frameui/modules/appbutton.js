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