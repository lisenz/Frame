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