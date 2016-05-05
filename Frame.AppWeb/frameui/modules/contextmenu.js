/**
* jQuery FrameUI 2.1.0
* 
* 作者：zlx
* 日期：2015.06.20
* 说明：ContextMenu 控件。
*/
(function ($) {
    $.ContextMenu = function (options) {
        return $.Frame.Do.call(null, "ContextMenu", arguments, { IsStatic: true });
    };

    $.fn.ContextMenu = function (options) {
        options = $.extend({}, $.Defaults.ContextMenu, options || {});

        options.Target = options.Target || $(this);

        this.each(function () {
            var id = $(this).attr("dependOn");
            if (!id) {
                $.ContextMenu(options);
            }

            $(this).on("contextmenu", function (e) {
                return false;
            }).on("mousedown", function (e) {
                if (3 == e.which) {
                    var contextmenu = $.Frame.Get($(this).attr("dependOn"));
                    if (!contextmenu)
                        return;
                    contextmenu.Show({
                        X: (e.offsetX || e.clientX - $(e.target).offset().left + window.pageXOffset),
                        Y: (e.offsetY || e.clientY - $(e.target).offset().top + window.pageYOffset)
                    });
                }
            }).on("click", function (e) {
                var contextmenu = $.Frame.Get($(this).attr("dependOn"));
                if (!contextmenu)
                    return;
                contextmenu.Close();
            });
        });

        return $.Frame.Get(this, "dependOn");
    };

    $.Methods.ContextMenu = $.Methods.ContextMenu || {};

    $.Defaults.ContextMenu = {
        Target: null,
        Width: null,
        Mode: "primary",
        Items: [],    // {Text,HasChild,Childs:[],Icon,IconColor,Divider,Disabled,Actived,OnClick}
        X: 0,
        Y: 0,
        OnItemClick: null       // 该事件若 return false，则会忽略最前端的click隐藏菜单事件
    };

    $.Frame.Controls.ContextMenu = function (options) {
        $.Frame.Controls.ContextMenu.Parent.constructor.call(this, null, options);
    };

    $.Frame.Controls.ContextMenu.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "ContextMenu";
        },
        _IdPrev: function () {
            return "ContextMenuFor";
        },
        _ExtendMethods: function () {
            return $.Methods.ContextMenu;
        },
        _Render: function () {
            var contextmenu = this, options = this.Options;

            contextmenu.Target = options.Target;
            contextmenu.Target.attr("dependOn", contextmenu.Id);

            contextmenu.Container = $("<div></div>").addClass("frame-dropdown")
                                                    .addClass("frame-dropdown-preview")
                                                    .appendTo(contextmenu.Target).hide();
            contextmenu.Menu = $("<ul></ul>").addClass("frame-dropdown-menu").addClass("frame-dropdown-" + options.Mode).appendTo(contextmenu.Container);

            contextmenu.Set(options);
        },
        _SetWidth: function (width) {
            var contextmenu = this, options = this.Options;
            if (width)
                contextmenu.Menu.width(width);
        },
        _SetX: function (x) {
            var contextmenu = this, options = this.Options;
            $(contextmenu.Container).css("left", x);
        },
        _SetY: function (y) {
            var contextmenu = this, options = this.Options;
            $(contextmenu.Container).css("top", y);

        },
        _SetItems: function (items) {
            ///<summary>
            /// 设置下拉菜单的菜单项
            ///</summary>
            ///<param name="items" type="array">菜单项对象集合.
            /// 菜单项对象结构[{Text,HasChild,Childs:[],Icon,IconColor,Divider,Disabled,Actived,OnClick}]</param>
            var contextmenu = this, options = this.Options;

            $(items).each(function (idx) {
                contextmenu._Visitor(1, idx, this, contextmenu.Menu);
            });
        },
        _Visitor: function (lv, idx, obj, parent) {
            ///<summary>
            /// 生成可访问的菜单项
            ///</summary>
            ///<param name="lv" type="number">菜单项所属的层级.层级数从1起始.</param>
            ///<param name="idx" type="number">菜单项在集合中的索引.索引值从0起始.</param>
            ///<param name="obj" type="object">菜单项对象</param>
            ///<param name="parent" type="selector">装载菜单项元素的元素容器</param>
            var contextmenu = this, options = this.Options;
            var item = $("<li></li>").appendTo(parent).attr("lv", "lv_" + lv + "_" + idx);
            if (obj.OnClick) {
                item.on("click", function () {
                    if (item.hasClass("disabled"))
                        return false;
                    return obj.OnClick.call(this, this, obj);
                });
            } else if (contextmenu.HasBind("ItemClick") && !obj.HasChild) {
                item.on("click", function () {
                    if (item.hasClass("disabled"))
                        return false;
                    return contextmenu.Trigger("ItemClick", [this, obj]);
                });
            }
            if (!obj.Divider) {
                if (obj.Disabled)
                    item.addClass("disabled");
                else if (obj.Actived && !obj.Disabled)
                    item.addClass("active");
                if (obj.HasChild) {
                    item.addClass("frame-dropdown-submenu");
                    contextmenu._AppendTermWithChild(item, obj.Icon, obj.IconColor, obj.Text);
                    var subMenu = $("<ul></ul>").addClass("frame-dropdown-menu")
                                                .addClass("frame-dropdown-" + options.Mode)
                                                .addClass("pull-right").appendTo(item);
                    $(obj.Childs).each(function (i) {
                        contextmenu._Visitor((lv + 1), i, this, subMenu);
                    });
                } else {
                    contextmenu._AppendTermNonChild(item, obj.Icon, obj.IconColor, obj.Text);
                    return;
                }
            } else {
                item.addClass("divider");
                return;
            }
        },
        _AppendTermNonChild: function (item, icon, color, text) {
            var term = $("<a></a>").attr("href", "#").appendTo(item);
            if (icon)
                $("<i></i>").addClass(icon).addClass(color).css("margin-right", "0.3em").appendTo(term);
            term.append(text);
        },
        _AppendTermWithChild: function (item, icon, color, text) {
            var term = $("<a></a>").attr("href", "#").addClass("clearfix").appendTo(item);
            if (icon)
                $("<i></i>").addClass(icon).addClass("pull-left").addClass(color).appendTo(term);
            term.append($("<span></span>").addClass("pull-left").text(text));
            term.append($("<i></i>").addClass("icon-caret-right").addClass("pull-right"));
        },
        Show: function (option) {
            var contextmenu = this;
            contextmenu.Set(option);
            contextmenu.Container.slideDown();
        },
        Close: function () {
            var contextmenu = this;
            contextmenu.Container.slideUp();
        },
        EnableItem: function (lv, idx) {
            var contextmenu = this, options = this.Options;
            $(contextmenu.Menu).find("li[lv='lv_" + lv + "_" + idx + "']").removeClass("disabled");
        },
        DisableItem: function (lv, idx) {
            var contextmenu = this, options = this.Options;
            $(contextmenu.Menu).find("li[lv='lv_" + lv + "_" + idx + "']").addClass("disabled");
        },
        DeactiveItem: function (lv, idx) {
            var contextmenu = this, options = this.Options;
            $(contextmenu.Menu).find("li[lv='lv_" + lv + "_" + idx + "']").removeClass("active");
        },
        ActiveItem: function (lv, idx) {
            var contextmenu = this, options = this.Options;
            $(contextmenu.Menu).find("li[lv='lv_" + lv + "_" + idx + "']").addClass("active");
        }
    });

})(jQuery);