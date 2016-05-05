/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.06
* 说明：Navigation 菜单栏控件。
*/
(function ($) {
    $.fn.Navigation = function (options) {
        return $.Frame.Do.call(this, "Navigation", arguments);
    };

    $.Methods.Navigation = $.Methods.Navigation || {};

    $.Defaults.Navigation = {
        Items: null,       // [{Icon,Title,Badge,Children:[{Title}]}]
        Url: null,
        Params: null,
        Method: "POST",
        DataType: "json",
        AutoExpand: true,
        OnHeaderClick: null,
        OnChildirenClick: null,
        OnResolve: null,
        OnBeforeExpand: null,
        OnExpand: null
    };

    $.Frame.Controls.Navigation = function (element, options) {
        $.Frame.Controls.Navigation.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Navigation.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Navigation";
        },
        _IdPrev: function () {
            return "NavigationFor";
        },
        _ExtenMethods: function () {
            return $.Methods.Navigation;
        },
        _PreRender: function () {
            var navigation = this, options = this.Options;
            if (!$(navigation.Element).hasClass("frame-navigation")) {
                $(navigation.Element).addClass("frame-navigation");
            }

            navigation.Children = [];
        },
        _Render: function () {
            var navigation = this, options = this.Options;

            navigation.Set(options);
        },
        _SetItems: function (items) {
            var navigation = this, options = this.Options;
            navigation.Children.length = 0;

            if (items && items.length > 0) {
                $(items).each(function () {
                    navigation.Children.push(this);
                    navigation._CreateHeader(this);
                });

            }
        },
        _CreateHeader: function (item) {
            var navigation = this, options = this.Options;

            var head = $("<li></li>").appendTo(navigation.Element).on("click", function () {
                var index = $(navigation.Element).children().index(this);
                navigation.Expand(index);
                return false;
            });
            var button = $("<a></a>").appendTo(head);

            item.Icon && $("<i></i>").addClass(item.Icon).appendTo(button);
            $("<span></span>").addClass("menu-text").text(item.Title).appendTo(button);
            if (item.Children && item.Children.length > 0) {
                button.addClass("dropdown-toggle");
                $("<b></b>").addClass("arrow").addClass("icon-angle-down").appendTo(button);
                var submenu = $("<ul></ul>").addClass("submenu").appendTo(head);
                navigation._CreateChildrenItem(submenu, item.Children);
            }
        },
        _CreateChildrenItem: function (header, childs) {
            var navigation = this, options = this.Options;
            if (childs && childs.length > 0) {
                $(childs).each(function () {
                    var item = $("<li></li>").appendTo(header).on("click", function () {
                        $(".active", navigation.Element).removeClass("active");
                        var parent = $(header).parent();
                        var parentIndex = $(navigation.Element).children().index(parent[0]);
                        var index = $(header).children().index(this);
                        parent.addClass("active");
                        $(this).addClass("active");
                        if (navigation.HasBind("ChildirenClick"))
                            navigation.Trigger("ChildirenClick", [this, navigation.Children[parentIndex], navigation.Children[parentIndex].Children[index]]);
                        return false;
                    });
                    var button = $("<a></a>").append($("<i></i>").addClass("icon-double-angle-right")).appendTo(item);
                    $("<span></span>").addClass("menu-text").text(this.Title).appendTo(button);
                });
            }
        },
        Expand: function (which) {
            var navigation = this, options = this.Options;

            var index = (typeof which == "number") ? which : navigation._GetHeaderIndex(which);
            if (index < 0)
                return;

            var header = $(navigation.Element).children().eq(index);
            if ($(".submenu", header).length > 0) {
                var expand = true;
                if (navigation.HasBind("BeforeExpand"))
                    expand = navigation.Trigger("BeforeExpand", [header, navigation.Children[index]]);
                if (expand) {
                    if (options.AutoExpand) {
                        $(navigation.Element).children(".open").not(header).removeClass("open");
                    }
                    $(header).toggleClass("open");
                    if (navigation.HasBind("Expand"))
                        navigation.Trigger("Expand", [header, navigation.Children[index]]);
                }
            }
            else {
                $(".active", navigation.Element).removeClass("active");
                $(".open", navigation.Element).removeClass("open");
                $(header).addClass("active");
                if (navigation.HasBind("HeaderClick"))
                    navigation.Trigger("HeaderClick", [header, navigation.Children[index]]);
            }
        },
        _GetHeaderIndex: function (text) {
            var navigation = this, options = this.Options;
            var index = -1;
            $(options.Items).each(function (i) {
                if (this.Title == text)
                    index = i;
            });

            return index;
        },
        _SetUrl: function (url) {
            var navigation = this, options = this.Options;
            if (url != null && options.Items == null) {
                $.ajax({
                    dataType: options.DataType, type: options.Method,
                    url: url,
                    data: options.Params,
                    success: function (datas) {
                        navigation.LoadData(datas);
                    },
                    error: function (xhr, info, exp) {
                        // 三个参数:XMLHttpRequest对象、错误信息、(可选)捕获的异常对象
                    },
                    complete: function (xhr, ts) {
                    }
                });
            }
        },
        LoadData: function (datas) {
            var navigation = this, options = this.Options;
            var nodes = datas;
            if (navigation.HasBind("Resolve"))
                nodes = navigation.Trigger("Resolve", [datas]);
            navigation.Set({ Items: nodes });
        }
    });


})(jQuery);