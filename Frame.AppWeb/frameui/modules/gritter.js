/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.15
* 说明：Gritter 平铺控件。
*/
(function () {
    $.Gritter = function (options) {
        return $.Frame.Do.call(null, "Gritter", arguments, { IsStatic: true });
    };

    $.Methods.Gritter = $.Methods.Gritter || {};

    $.Defaults.Gritter = {}; // item{Title,Text,Image,Sticky,Time,Cls[info,error,success,warning,light]}

    $.Frame.Controls.Gritter = function (options) {
        $.Frame.Controls.Gritter.Parent.constructor.call(this, $("<div></div>")[0], options);
    };

    $.Frame.Controls.Gritter.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Gritter";
        },
        _IdPrev: function () {
            return "GritterFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Gritter;
        },
        _Render: function () {
            var gritter = this, options = this.Options;

            var g = $("body").find(".frame-gritter");
            if (g.length > 0)
                gritter.Element = g[0];
            else {
                $(gritter.Element).appendTo("body");
                $(gritter.Element).addClass("frame-gritter");
            }

            gritter._Custom_Timer = 0;
            gritter._Item_Count = 0;
        },
        Add: function (item) {
            var gritter = this, options = this.Options;
            gritter._Item_Count++;

            var wrapper = $("<div></div>").addClass("frame-gritter-item-wrapper")
                .attr("id", "gritter-item-wrapper" + gritter._Item_Count).attr("idx", gritter._Item_Count);

            if (item.Cls)
                wrapper.addClass("gritter-" + item.Cls);
            $(gritter.Element).append(wrapper);

            var itemcontainer = $("<div></div>").addClass("frame-gritter-item").appendTo(wrapper)
                .append($("<a></a>").addClass("frame-gritter-close").attr("idx", gritter._Item_Count).click(function () {
                    gritter.Remove($(this).attr("idx"));
                }));

            var content;
            if (item.Image) {
                var img = $("<img />").addClass("frame-gritter-image").attr("src", item.Image);
                content = $("<div></div>").addClass("frame-gritter-with-image");
                itemcontainer.append(img);
            } else {
                content = $("<div></div>").addClass("frame-gritter-without-image");
            }

            if (item.Title)
                content.append($("<span></span>").addClass("frame-gritter-title").text(item.Title || ""));
            content.append($("<p></p>").text(item.Text));
            itemcontainer.append(content);
            itemcontainer.append($("<div></div>").css("clear", "both"));
            if (item.Time)
                gritter._Custom_Timer = item.Time;

            wrapper.fadeIn("fast");

            // 在指定时间后自动移除
            if (!item.Sticky && item.Time) {
                gritter._SetFadeTimer(wrapper, gritter._Item_Count);
            }
        },
        _SetFadeTimer: function (g, d) {
            var gritter = this, options = this.Options;

            setTimeout(function () {
                gritter.Remove($(g).attr("idx"));
            },
            gritter._Custom_Timer);
        },
        Remove: function (index) {
            var gritter = this, options = this.Options;
            $(gritter.Element).children("#gritter-item-wrapper" + index).fadeOut("slow", function () {
                $(this).remove();
                if ($(gritter.Element).children().length == 0) {
                    $.Frame.Remove(gritter);
                    $(gritter.Element).remove();
                }
            });

        },
        Clear: function () {
            var gritter = this, options = this.Options;
            $(gritter.Element).empty();
            $.Frame.Remove(gritter);
        }
    });

    $.Gritter.Add = function (item) {
        var result = $.Frame.Find("Gritter");
        var gritter;
        if (result.length == 0)
            gritter = $.Gritter();
        else
            gritter = result[0];

        gritter.Add(item);
    };

    $.Gritter.Clear = function () {
        var result = $.Frame.Find("Gritter");
        var gritter;
        if (result.length == 0)
            gritter = $.Gritter();
        else
            gritter = result[0];

        gritter.Clear();
    };

})(jQuery);