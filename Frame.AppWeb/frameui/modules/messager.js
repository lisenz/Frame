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
