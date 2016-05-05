/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.04
* 说明：NavBar 导航栏控件。
*/
(function ($) {
    $.fn.NavBar = function (options) {
        return $.Frame.Do.call(this, "NavBar", arguments);
    };

    $.Methods.NavBar = $.Methods.NavBar || {};

    $.Defaults.NavBar = {
        Brand: null,    // 导航栏左侧logo图标及产品名称区域:{Icon,Text}
        NavItems: [],   // 导航栏工具项 {Icon,Text,Badge,Color,DropMenu:{Header:{Icon,Title},Footer,OnLastItemClick,Children:[{Content,OnItemStyle}]}}
        UserMenu: null,  // 导航栏下拉菜单项 {Photo,Content,Items:[{Icon,Text,Divider}]}
        OnBrandClick: null,
        OnBeforeItemOpen: null,
        OnNavItemClick: null,
        OnNavItemChildClick: null,
        OnBefornUserMenuOpen: null,
        OnUserMenuItemClick: null
    };

    $.Frame.Controls.NavBar = function (element, options) {
        $.Frame.Controls.NavBar.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.NavBar.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "NavBar";
        },
        _IdPrev: function () {
            return "NavBarFor";
        },
        _ExtendMethods: function () {
            return $.Methods.NavBar;
        },
        _PreRender: function () {
            var navbar = this;

            if (!$(navbar.Element).hasClass("frame-navbar")) {
                $(navbar.Element).addClass("frame-navbar");
            }
            navbar.Children = [];
        },
        _Render: function () {
            var navbar = this, options = this.Options;

            navbar.Container = $("<div></div>").addClass("frame-navbar-container").appendTo(navbar.Element);
            navbar.BrandContainer = $("<div></div>").addClass("frame-navbar-header")
                                                    .addClass("pull-left")
                                                    .appendTo(navbar.Container);
            navbar.Brand = $("<a></a>").addClass("frame-navbar-brand").append("<small></small>").appendTo(navbar.BrandContainer);

            navbar.NavContainer = $("<div></div>").addClass("frame-navbar-header")
                                                  .addClass("pull-right")
                                                  .appendTo(navbar.Container);
            navbar.NavDropMenu = $("<ul></ul>").addClass("frame-navbar-nav").appendTo(navbar.NavContainer);

            navbar.Set(options);
        },
        _SetBrand: function (brand) {
            var navbar = this, options = this.Options;
            if (brand) {
                if (brand.Icon) {
                    var icon = $("<i></i>").addClass(brand.Icon);
                    $("small", navbar.Brand).append(icon);
                }
                $("small", navbar.Brand).append($("<span></span>").text(brand.Text));
                if (navbar.HasBind("BrandClick")) {
                    navbar.Brand.on("click", function () {
                        navbar.Trigger("BrandClick", [this])
                        return false;
                    });
                }
            }
        },
        _SetNavItems: function (navitems) {
            ///<summary>
            /// 设置导航栏列表项及下属菜单
            ///</summary>
            ///<param name="navitems" type="array">列表项对象集合,
            /// 对象结构：{Icon,Text,Badge,Color,DropMenu},
            /// 其中DropMenu为菜单项对象集合，
            /// 对象结构：{Header:{Icon,Title},Footer,OnLastItemClick,Children:[{Content,OnItemStyle}]},
            /// 内置Color可选：grey,purple,green,
            ///</param>
            var navbar = this, options = this.Options;

            $(navitems).each(function () {
                var item = navbar._CreateNavItem(this);
                navbar.NavDropMenu.append(item);
                if (this.DropMenu) {
                    navbar._CreateNavItemDropMenu(item, this.DropMenu);
                }
            });
        },
        _CreateNavItem: function (navitem) {
            ///<summary>
            /// 创建导航栏的列表项，这里不创建项所属菜单
            ///</summary>
            ///<param name="navitem" type="object">列表项对象:{Icon,Text,Badge,Color}</param>
            var navbar = this, options = this.Options;

            navbar.Children.push(navitem);

            var item = $("<li></li>").on("click", function () {
                if ($(this).hasClass("disabled"))
                    return false;
                var isOpen = true;
                var idx = navbar.NavDropMenu.children().index(this);

                // BeforeItemOpen接收参数:当前点击的元素对象,元素对象对应的数据对象,数据对象在对象集合中的索引位置
                if (navbar.HasBind("BeforeItemOpen") && !$(this).hasClass("open"))
                    isOpen = navbar.Trigger("BeforeItemOpen", [$("a.dropdown-toggle", this), navbar.Children[idx], idx]);
                if (isOpen && !$(this).hasClass("open")) {
                    $("li.open", navbar.NavDropMenu).removeClass("open");
                    $(this).addClass("open");

                    // NavItemClick接收参数:当前点击的元素对象,元素对象对应的数据对象,数据对象在对象集合中的索引位置
                    if (navbar.HasBind("NavItemClick"))
                        navbar.Trigger("NavItemClick", [$("a.dropdown-toggle", this), navbar.Children[idx], idx]);
                    return false;
                } else
                    $(this).removeClass("open");
            });
            var dropdownbutton = $("<a></a>").addClass("dropdown-toggle").appendTo(item);

            navitem.Color && item.addClass(navitem.Color);
            navitem.Icon && dropdownbutton.append($("<i></i>").addClass(navitem.Icon));

            if (navitem.Text) {
                if (!navitem.Badge)
                    dropdownbutton.append(navbar._WrapNavItemText(navitem.Text));
                else
                    dropdownbutton.append(navbar._WrapNavItemBadge(navitem.Text, navitem.Badge));
            }
            return item;
        },
        _WrapNavItemText: function (text) {
            var navbar = this, options = this.Options;

            return $("<span></span>").text(text);
        },
        _WrapNavItemBadge: function (text, badge) {
            var navbar = this, options = this.Options;

            return $("<span></span>").addClass("frame-badge").addClass("frame-badge-" + badge).text(text);
        },
        _CreateNavItemDropMenu: function (navitem, dropmenu) {
            ///<summary>
            /// 创建导航栏的工具项所属菜单
            ///</summary>
            ///<param name="navitem" type="object">工具项对象</param>
            ///<param name="dropmenu" type="object">工具项菜单的菜单项集合对象
            /// {Header:{Icon,Title},Footer:{Icon,Text},OnLastItemClick,Children:[{Content,OnItemStyle}]}</param>
            var navbar = this, options = this.Options;
            if (dropmenu.Children.length > 0) {
                var menu = $("<ul></ul>").addClass("pull-right").addClass("frame-navbar-dropdown-menu");
                var header = $("<li></li>").addClass("frame-navbar-dropdown-header").appendTo(menu);
                dropmenu.Header.Icon && $("<i></i>").addClass(dropmenu.Header.Icon).appendTo(header);
                $("<span></span>").text(dropmenu.Header.Title).appendTo(header);

                $(dropmenu.Children).each(function () {
                    var item = navbar._CreateDropMenuItem(this);
                    menu.append(item);
                });

                if (dropmenu.Footer) {
                    var last = $("<li></li>").addClass("last-child").append("<a></a>").appendTo(menu)
                        .on("click", function () {
                            if (dropmenu.OnLastItemClick) {
                                var idx = navbar.NavDropMenu.children().index(navitem[0]);

                                // LastItemClick接收参数:当前菜单所属导航栏数据对象,数据对象在导航列表中的索引位置
                                dropmenu.OnLastItemClick.call(this, navbar.Children[idx], idx);
                                return false;
                            }
                        });
                    $("a", last).append(dropmenu.Footer.Text);
                    dropmenu.Footer.Icon && $("<i></i>").addClass(dropmenu.Footer.Icon).appendTo($("a", last));
                }

                navitem.append(menu);
            }
        },
        _CreateDropMenuItem: function (dropmenuitem) {
            var navbar = this, options = this.Options;
            var item = $("<li></li>").append("<a></a>").on("click", function () {
                if ($(this).hasClass("disabled"))
                    return false;

                if (navbar.HasBind("NavItemChildClick")) {
                    var menu = $(this).parent();
                    var idx = menu.children().index(this); // 这里索引值从1起始，因为需要排除菜单标题

                    // NavItemChildClick接收参数:当前点击的元素对象,元素对象在菜单列表中的索引位置
                    navbar.Trigger("NavItemChildClick", [$(this).first(), idx]);
                }
                $("li.open", navbar.NavDropMenu).removeClass("open");
                return false;
            });
            if (dropmenuitem.OnItemStyle) {
                dropmenuitem.Content = dropmenuitem.OnItemStyle.call(this, dropmenuitem.Content);
            }
            $("a", item).append(dropmenuitem.Content);
            return item;
        },
        _SetUserMenu: function (usermenu) {
            var navbar = this, options = this.Options;

            if (usermenu) {
                var item = $("<li></li>").addClass("light-blue").addClass("usermenu").appendTo(navbar.NavDropMenu)
                .on("click", function () {
                    var isOpen = true;
                    if (navbar.HasBind("BefornUserMenuOpen"))
                        isOpen = navbar.Trigger("BefornUserMenuOpen", []);
                    if (isOpen && !$(this).hasClass("open")) {
                        $("li.open", navbar.NavDropMenu).removeClass("open");
                        $(this).addClass("open");
                    } else
                        $(this).removeClass("open");
                });
                var dropdownbutton = $("<a></a>").addClass("dropdown-toggle").appendTo(item);

                usermenu.Photo && $("<i></i>").addClass("nav-user-photo").addClass(usermenu.Photo).appendTo(dropdownbutton);
                $("<span></span>").addClass("user-info").append(usermenu.Content).appendTo(dropdownbutton);
                dropdownbutton.append($("<i></i>").addClass("icon-caret-down"));

                if (usermenu.Items.length > 0) {
                    var menu = $("<ul></ul>").addClass("pull-right").addClass("frame-dropdown-menu").appendTo(item);
                    $(usermenu.Items).each(function () {
                        if (!this.Divider) {
                            var child = $("<li></li>").append("<a></a>").appendTo(menu).on("click", function () {
                                if ($(this).hasClass("disabled"))
                                    return false;

                                if (navbar.HasBind("UserMenuItemClick"))
                                    navbar.Trigger("UserMenuItemClick", [$("span", this).text()]);
                                $("li.open", navbar.NavDropMenu).removeClass("open");
                                return false;
                            });
                            this.Icon && $("<i></i>").addClass(this.Icon).appendTo($("a", child));
                            $("a", child).append($("<span></span>").text(this.Text));
                        }
                        else
                            $("<li></li>").addClass("divider").appendTo(menu)
                    });
                }
            }
        },
        AddNavItem: function (navitem) {
            ///<summary>
            /// 追加一个导航栏列表项
            ///</summary>
            ///<param name="navitem" type="object">列表项对象:{Icon,Text,Badge,Color}</param>
            var navbar = this, options = this.Options;

            var item = navbar._CreateNavItem(navitem);

            var usermenu = $("li.usermenu", navbar.NavDropMenu);
            if (usermenu.length > 0)
                $("li.usermenu", navbar.NavDropMenu).before(item);
            else
                navbar.NavDropMenu.append(item);

            if (navitem.DropMenu) {
                navbar._CreateNavItemDropMenu(item, navitem.DropMenu);
            }

            return item;
        },
        AddNavItemDropMenu: function (index, dropmenu) {
            ///<summary>
            /// 创建导航栏的列表项所属菜单
            ///</summary>
            ///<param name="index" type="number">列表项在导航栏列表中的索引位置.索引值值从0起始.</param>
            ///<param name="dropmenu" type="object">列表项菜单的菜单项集合对象
            /// {Header:{Icon,Title},Footer:{Icon,Text},OnLastItemClick,Children:[{Content,OnItemStyle}]}</param>
            var navbar = this, options = this.Options;

            if (navbar.NavDropMenu.children().length > 0) {
                var item = navbar.NavDropMenu.children().eq(index);
                if (item.length > 0)
                    navbar._CreateNavItemDropMenu(item, dropmenu);
            }
        },
        AddNavItemDropMenuItem: function (index, dropmenuitems) {
            ///<summary>
            /// 创建导航栏的列表项所属菜单的子项
            ///</summary>
            ///<param name="index" type="number">列表项在导航栏列表中的索引位置.索引值值从0起始.</param>
            ///<param name="dropmenuitems" type="array">列表项菜单的菜单项集合对象[{Content,OnItemStyle}]</param>
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var item = navbar.NavDropMenu.children().eq(index);
                if (item.length > 0) {
                    var menu = $(".frame-navbar-dropdown-menu", item);
                    var last = $("li.last-child", menu);
                    $(dropmenuitems).each(function () {
                        var menuitem = navbar._CreateDropMenuItem(this);
                        if (last.length > 0)
                            last.before(menuitem);
                        else
                            menu.append(menuitem);
                    });
                }
            }
        },
        SetNavItemText: function (index, text) {
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var item = navbar.NavDropMenu.children().eq(index);
                if (item.length > 0) {
                    $("a>span", item).text(text);
                    navbar.Children[index].Text = text;
                }
            }
        },
        DisableNavItem: function (index) {
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var item = navbar.NavDropMenu.children().eq(index);
                if (item.length > 0)
                    item.addClass("disabled");
            }
        },
        EnableNavItem: function (index) {
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var item = navbar.NavDropMenu.children().eq(index);
                if (item.length > 0) {
                    item.removeClass("disabled");
                }
            }
        },
        DisableNavDropMenuItem: function (parent, index) {
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var item = navbar.NavDropMenu.children().eq(parent);
                if (item.length > 0) {
                    var menu = $(".frame-navbar-dropdown-menu", item);
                    if (menu.children().length > 0) {
                        var menuitem = menu.children().eq(index);
                        menuitem.length > 0 && menuitem.addClass("disabled");
                    }
                }
            }
        },
        EnableNavDropMenuItem: function (parent, index) {
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var item = navbar.NavDropMenu.children().eq(parent);
                if (item.length > 0) {
                    var menu = $(".frame-navbar-dropdown-menu", item);
                    if (menu.children().length > 0) {
                        var menuitem = menu.children().eq(index);
                        menuitem.length > 0 && menuitem.removeClass("disabled");
                    }
                }
            }
        },
        DisableUserMenu: function () {
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var menu = $(".usermenu", navbar.NavDropMenu);
                if (menu.length > 0) {
                    menu.addClass("disabled");
                }
            }
        },
        EnableUserMenu: function () {
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var menu = $(".usermenu", navbar.NavDropMenu);
                if (menu.length > 0) {
                    menu.removeClass("disabled");
                }
            }
        },
        DisableUserMenuItem: function (index) {
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var menu = $(".usermenu", navbar.NavDropMenu);
                if (menu.length > 0 && menu.children().length > 0) {
                    menu.children().eq(index).addClass("disabled");
                }
            }
        },
        EnableUserMenuItem: function (index) {
            var navbar = this, options = this.Options;
            if (navbar.NavDropMenu.children().length > 0) {
                var menu = $(".usermenu", navbar.NavDropMenu);
                if (menu.length > 0 && menu.children().length > 0) {
                    menu.children().eq(index).removeClass("disabled");
                }
            }
        }

    });


})(jQuery);