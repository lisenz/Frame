/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.20
* 说明：BreadCrumbs 面包屑控件。
*/
(function ($) {
    $.fn.BreadCrumbs = function (options) {
        return $.Frame.Do.call(this, "BreadCrumbs", arguments);
    };

    $.Methods.BreadCrumbs = $.Methods.BreadCrumbs || {};

    $.Defaults.BreadCrumbs = {
        Items: [],         //{Url,Params,Text}
        ToolBar: null,
        OnItemClick: null
    };

    $.Frame.Controls.BreadCrumbs = function (element, options) {
        $.Frame.Controls.BreadCrumbs.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.BreadCrumbs.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "BreadCrumbs";
        },
        _IdPrev: function () {
            return "BreadCrumbsFor";
        },
        _ExtendMethods: function () {
            return $.Methods.BreadCrumbs;
        },
        _PreRender: function () {
            var breadcrumbs = this;

            if (!$(breadcrumbs.Element).hasClass("frame-breadcrumbs")) {
                $(breadcrumbs.Element).addClass("frame-breadcrumbs");
            }
        },
        _Render: function () {
            var breadcrumbs = this, options = this.Options;
            breadcrumbs.Container = $("<ul></ul>").addClass("frame-breadcrumb").appendTo(breadcrumbs.Element);

            breadcrumbs.Set(options);
        },
        _SetItems: function (items) {
            var breadcrumbs = this, options = this.Options;

            if (items && items.length > 0) {
                $(items).each(function (i) {
                    breadcrumbs._Append(i, this);
                });
                $(breadcrumbs.Container).children().last().find("a").trigger("click");
                $(breadcrumbs.Container).children().last().addClass("active");
            }
        },
        _SetToolBar: function (tools) {
            var breadcrumbs = this, options = this.Options;
            if (tools && tools.length > 0) {
                var toolbar = $("<div></div>").addClass("frame-breadcrumb-toolbar").appendTo(breadcrumbs.Element);
                //                $(tools).each(function () {
                //                    toolbar.append($(this.Element));
                //                });
                $(tools).each(function () {
                    switch (this.Type) {
                        case "Button":
                            var button = $("<button></button>").Button({
                                Mode: "transparent",
                                Size: "large",
                                Icon: this.Icon,
                                IconSite: "only",
                                OnClick: this.OnClick
                            });
                            toolbar.append(button.Element);
                            this.Desc && $(button.Element).Tip({ Placement: "auto", Mode: "inverse", Title: this.Desc });
                            break;
                        case "TextBox":
                            var textbox = $('<input type="text" />').TextBox({
                                Icon: this.Icon,
                                Size: "small",
                                Width: this.Width,
                                OnBeforeFocus: this.OnBeforeFocus,
                                OnFocus: this.OnFocus,
                                OnBlur: this.OnBlur,
                                OnChange: this.OnChange
                            });
                            toolbar.append($(textbox.Element).parent());
                            this.Desc && $(textbox.Element).parent().Tip({ Placement: "auto", Mode: "inverse", Title: this.Desc });
                            break;
                    }
                });
            }
        },
        _Join: function (url, params) {
            if (url) {
                url += url.indexOf('?') == -1 ? "?" : "&";
                for (var name in params) {
                    url += (name + "=" + params[name] + "&");
                }
                url += ("random=" + new Date().getTime());
            } else
                url = "#";
            return url;
        },
        Append: function (item) {
            var breadcrumbs = this, options = this.Options;
            var index = options.Items.length;
            options.Items.push(item);
            $("li.active", breadcrumbs.Container).removeClass("active");
            breadcrumbs._Append(index, item);
            $(breadcrumbs.Container).children().last().find("a").trigger("click");
            $(breadcrumbs.Container).children().last().addClass("active");
        },
        AppendRange: function (items) {
            var breadcrumbs = this, options = this.Options;
            if (items && items.length > 0) {
                $("li.active", breadcrumbs.Container).removeClass("active");
                $(items).each(function () {
                    var index = options.Items.length;
                    options.Items.push(this);
                    breadcrumbs._Append(index, this);
                });
                $(breadcrumbs.Container).children().last().find("a").trigger("click");
                $(breadcrumbs.Container).children().last().addClass("active");
            }
        },
        _Append: function (index, item) {
            var breadcrumbs = this, options = this.Options;

            var itemElement = $("<li></li>").appendTo(breadcrumbs.Container);
            var url = breadcrumbs._Join(item.Url, item.Params);
            if (index == 0) {
                $("<i></i>").addClass("icon-home").addClass("home-icon").appendTo(itemElement);
            }
            $("<a></a>").attr("idx", index).text(item.Text).attr("href", url).appendTo(itemElement)
                            .on("click", function (e) {
                                if ($(this).parent().hasClass("active"))
                                    return false;
                                var r = true;
                                if (breadcrumbs.HasBind("ItemClick")) {
                                    var index = parseInt($(this).attr("idx"));
                                    var d = options.Items[index];
                                    r = breadcrumbs.Trigger("ItemClick", [this, d]);
                                }
                                if (r)
                                    breadcrumbs._Remove(this, index);
                                return false;
                            });
        },
        _Remove: function (sender, index) {
            var breadcrumbs = this, options = this.Options;
            options.Items.splice(index + 1, options.Items.length - 1);

            $(breadcrumbs.Container).children().each(function (i) {
                if (i > index)
                    $(this).remove();
            });
            $(sender).parent().addClass("active");
        },
        Clear: function () {
            var breadcrumbs = this, options = this.Options;
            options.Items.length = 0;
            $(breadcrumbs.Container).empty();
        }
    });

})(jQuery);