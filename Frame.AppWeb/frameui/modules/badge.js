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