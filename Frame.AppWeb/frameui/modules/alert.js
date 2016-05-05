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