/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.06.06
* 说明：Tabs 标签页控件。
*/
(function ($) {

    $.fn.Tabs = function (options) {
        return $.Frame.Do.call(this, "Tabs", arguments);
    };

    $.Methods.Tabs = $.Methods.Tabs || {};

    $.Defaults.Tabs = {
        Tabs: [],          // 选项卡对象格式：{Title,Url,Content,Closable,Selected,Disabled,Icon:{color,cls}}
        Width: "auto",
        Height: "auto",
        Plain: false,       // 是否为简洁模式,即无边框
        OnLoad: null,
        OnSelect: null,
        OnBeforeClose: null,
        OnClose: null,
        OnAdd: null
    };

    $.Frame.Controls.Tabs = function (element, options) {
        $.Frame.Controls.Tabs.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Tabs.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Tabs";
        },
        _IdPrev: function () {
            return "TabsFor";
        },
        _ExtenMethods: function () {
            return $.Methods.Tabs;
        },
        _PreRender: function () {
            var tabs = this;
            if (!$(tabs.Element).hasClass("frame-tabs")) {
                $(tabs.Element).addClass("frame-tabs");
            }
            tabs.Children = [];
            tabs._ResetTabItemsExistsInElement();
        },
        _Render: function () {
            var tabs = this, options = this.Options;
            tabs.NavHeader = $("<div></div>").addClass("frame-tabs-nav-wrap");
            tabs.PrevButton = $("<i></i>").addClass("icon-angle-left").on("click", function () { tabs._Prev(); }).hide();
            tabs.NextButton = $("<i></i>").addClass("icon-angle-right").on("click", function () { tabs._Next(); }).hide();
            tabs.NavContainer = $("<div></div>").addClass("frame-tabs-nav-container");
            tabs.NavBar = $("<ul></ul>").addClass("frame-tabs-nav").appendTo(tabs.NavContainer);

            $(tabs.Element).append(tabs.NavHeader);
            $(tabs.NavHeader).append(tabs.PrevButton).append(tabs.NavContainer).append(tabs.NextButton);

            tabs.Contents = $("<div></div>").addClass("frame-tabs-content");
            if (options.Plain) {
                tabs.Contents.addClass("plain");
            }
            $(tabs.Element).append(tabs.Contents);

            tabs.Set(options);

        },
        _ResetTabItemsExistsInElement: function () {
            ///<summary>
            /// 将页面元素已存在的子元素追加到Options配置对象的Tabs属性中,以便在构造Tabs控件时使用
            ///</summary>
            var tabs = this, options = this.Options;

            if ($(tabs.Element).children().length > 0) {
                $(tabs.Element).children().each(function () {
                    var option = {
                        Title: $(this).attr("title"),
                        Url: null,
                        Content: $(this),
                        Disabled: $(this).attr("disabled"),
                        Selected: $(this).attr("selected"),
                        Closable: $(this).attr("closable"),
                        Icon: $(this).attr("icon") ? $.extend({}, (new Function("return {" + $(this).attr("icon") + "}"))()) : false
                    };

                    $(this).removeAttr("title").removeAttr("url").removeAttr("disabled")
                           .removeAttr("selected").removeAttr("closable").removeAttr("icon");
                    // 向Options配置的Tabs数组属性的开头添加包装好的配置项
                    tabs.Children.unshift(option);
                    $(this).remove();
                });
            }
        },
        _SetTabs: function (items) {
            ///<summary>
            /// 设置选项卡
            ///</summary>
            ///<param name="items" type="array">标签页对象数组集合.对象结构:[Title,Url,Content,Closable,Selected,Disable,Icon{cls,color}]</param>
            var tabs = this, options = this.Options;

            tabs.NavBarWidth = 0;
            $.merge(tabs.Children, items);
            $(tabs.Children).each(function (idx) {
                tabs._CreateTabItem(idx, this);
            });

            // 默认选中第一个选项卡
            var sender = tabs.NavBar.children().eq(0);
            if (sender.length > 0) {
                tabs._ToggleTab(sender);
            }
        },
        _CreateTabItemButton: function (idx, id, title, icon, closable) {
            var tabs = this;
            var item = $("<a></a>").attr("href", "#" + id).attr("idx", idx);
            if (icon) {
                var i = $("<i></i>").addClass(icon.color).addClass(icon.cls).addClass("bigger-110");
                item.append(i);
            }
            item.append(title);
            if (closable) {
                var btn = $("<i></i>").addClass("with-x").text("×")
                                .on("click", function () {
                                    if ($(this).parent().parent().hasClass("disabled"))
                                        return;
                                    var selected = $(this).parent().parent().hasClass("active");
                                    var idx = parseInt($(this).parent().attr("idx"));
                                    tabs.Close(idx);
                                    idx -= 1;
                                    if (idx >= 0 && selected) {
                                        tabs.SelectTab(idx);
                                    }
                                });
                item.append(btn);
            }
            return item;
        },
        _CreateTabItemContent: function (idx, id, url, content) {
            var tabContent = $("<div></div>").attr("id", id).attr("idx", idx).addClass("tab-panel");
            if (url) {
                var framename = "Frame" + new Date().getTime();
                var iframe = $('<iframe></iframe>').attr("id", framename)
                .attr("name", framename).attr("src", url).attr("frameborder", "0").attr("scrolling", "no");

                tabContent.append(iframe);
            } else {
                tabContent.append(content);
            }

            return tabContent;
        },
        _CreateTabItemId: function (idx) {
            var tabs = this;
            return tabs.Id + "_ti_" + idx;
        },
        _SetWidth: function (width) {
            var tabs = this, options = this.Options;

            if (width != "auto") {
                $(tabs.Element).width(width);
            }

            tabs._ToggleButton();
        },
        _SetHeight: function (height) {
            var tabs = this, options = this.Options;
            if (height != "auto") {
                var wrapHeight = tabs.NavHeader.height();
                var diff = options.Plain ? 0 : 2;
                $(tabs.Element).height(height);
                tabs.Contents.height(height - wrapHeight - diff);
            } else {
                tabs.Contents.addClass("no-overflow");
            }
        },
        _ToggleButton: function () {
            var tabs = this;
            var wrapWidth = tabs.NavHeader.width();
            if (tabs.NavBarWidth > wrapWidth) {
                tabs.PrevButton.show();
                tabs.NextButton.show();
                tabs.NavContainer.width(wrapWidth - 26);
                tabs.NavBar.children().first().css("margin-left", "0px");
            } else {
                tabs.PrevButton.hide();
                tabs.NextButton.hide();
                tabs.NavContainer.width(wrapWidth + 26);
                tabs.NavBar.children().first().css("margin-left", "5px");
                tabs.NavBar.animate({ left: 0 });
            }
        },
        _GetPrevMoveList: function () {
            ///<summary>
            /// 记录每个项的left,由左到右
            ///</summary>
            var tabs = this;
            var list = new Array();
            tabs.NavBar.children().each(function (i) {
                var current = 0;
                if (i > 0) {
                    current = parseInt(list[i - 1]) + $(this).prev().width();
                }
                list.push(current);
            });

            return list;
        },
        _GetNextMoveList: function () {
            ///<summary>
            /// 记录每个项的left,由右到左
            ///</summary>
            var tabs = this;
            var list = new Array();
            var barWidth = tabs.NavContainer.width();

            var itemLength = tabs.NavBar.children().length;
            for (var i = itemLength - 1; i >= 0; i--) {
                var current = tabs.NavBarWidth - barWidth + 2;
                if (i != itemLength - 1) {
                    current = parseInt(list[itemLength - 2 - i]) - tabs.NavBar.children().eq(i + 1).width();
                }
                list.push(current);
            }
            return list;
        },
        _Prev: function () {
            var tabs = this;
            var leftList = tabs._GetPrevMoveList();
            var currentLeft = -1 * parseInt(tabs.NavBar.css("left"));
            for (var i = 0; i < leftList.length - 1; i++) {
                if (leftList[i] < currentLeft && leftList[i + 1] >= currentLeft) {
                    tabs.NavBar.animate({ left: -1 * parseInt(leftList[i]) });
                    return;
                }
            }
        },
        _Next: function () {
            var tabs = this;
            var rightList = tabs._GetNextMoveList();

            var currentRight = -1 * parseInt(tabs.NavBar.css("left"));
            for (var i = 1; i < rightList.length; i++) {
                if (rightList[i] <= currentRight && rightList[i - 1] > currentRight) {
                    tabs.NavBar.animate({ left: -1 * parseInt(rightList[i - 1]) });
                    return;
                }
            }
        },
        SelectTab: function (which) {
            ///<summary>
            /// 选择指定的选项卡
            ///</summary>
            ///<param name="which" type="which">选项卡的索引值或名称</param>
            var tabs = this, options = this.Options;

            if (!tabs.Exists(which))
                return;

            var index = (typeof which == "number") ? which : tabs.GetTabIndex(which);
            var sender = tabs.NavBar.children().eq(index);

            if (tabs.HasBind("Select"))
                tabs.Trigger("Select", [sender, index]);

            if (tabs.Children[index].Selected)
                return tabs.Children[index];

            if (sender.length > 0) {
                if (tabs.HasBind("Load") && tabs.Children[index].Url)
                    tabs.Trigger("Load", [sender, $("iframe", tabs.Contents.children().eq(index))]);
                tabs._ToggleTab(sender);
                return tabs.Children[index];
            }
            else
                return;
        },
        Add: function (tab) {
            ///<summary>
            /// 添加一个选项卡
            ///</summary>
            ///<param name="tab" type="tab">选项卡对象</param>
            var tabs = this, options = this.Options;

            if (tabs.Exists(tab.Title))
                return;

            //            this.Options.Tabs.push(tab);
            //            var idx = this.Options.Tabs.length - 1;
            tabs.Children.push(tab);
            var idx = tabs.Children.length - 1;

            tabs._CreateTabItem(idx, tab);
            tabs._ToggleButton();
            tabs.SelectTab(idx);

            if (tabs.HasBind("Add"))
                tabs.Trigger("Add", [tabs.NavBar.children().eq(idx), idx]);
        },
        AddRange: function (items) {
            ///<summary>
            /// 添加多个选项卡
            ///</summary>
            ///<param name="items" type="array">选项卡对象集合</param>
            var tabs = this, options = this.Options;

            $(items).each(function () {
                if (!tabs.Exists(this.Title)) {
                    tabs.Children.push(this);
                    var idx = tabs.Children.length - 1;
                    tabs._CreateTabItem(idx, this);
                }
            });

            tabs._ToggleButton();
            tabs.SelectTab(tabs.Children.length - 1);
        },
        _CreateTabItem: function (idx, data) {
            var tabs = this, options = this.Options;
            var id = tabs._CreateTabItemId(idx);

            var tab = $("<li></li>").attr("idx", idx).on("click", function (e) {
                tabs._ToggleTab(this);
                return false;
            });

            var item = tabs._CreateTabItemButton(idx, id, data.Title, data.Icon, data.Closable);
            if (data.Selected) {
                tab.addClass("active");
            } else
                data.Selected = false;
            if (data.Disabled) {
                tab.addClass("disabled");
            }
            else
                data.Disabled = false;
            tab.append(item);
            tabs.NavBar.append(tab);
            tabs.NavBarWidth += tab.outerWidth(true); // 这里合计标签页头的实际宽度
            tabs.Contents.append(tabs._CreateTabItemContent(idx, id, data.Url, $(data.Content)));
        },
        _ToggleTab: function (tab) {
            var tabs = this, options = this.Options;

            var activeTab = $("li.active", tabs.NavBar);
            if (activeTab.length > 0) {
                activeTab.removeClass("active");
                tabs.Children[activeTab.attr("idx")].Selected = false;
            }
            $(tab).addClass("active");
            tabs.Children[$(tab).attr("idx")].Selected = true;

            $(tabs.Contents).children(".active").removeClass("active");
            var panel = $($("a", tab).attr("href"));
            panel.addClass("active");

            var iframe = $($("a", tab).attr("href")).children("iframe");
            if (iframe.length > 0) {
                var doc = iframe[0].contentDocument;
                if (doc != null) {
                    panel.height(doc.body.scrollHeight);
                }
            }
        },
        Exists: function (which) {
            ///<summary>
            /// 返回一个值，该值标识是否已存在指定索引或名称的选项卡
            ///</summary>
            ///<param name="which" type="which">选项卡的索引值或名称</param>
            var tabs = this, options = this.Options;
            var exists = false;

            if (tabs.Children.length == 0)
                return false;

            if (typeof which == "number") {
                if (which < 0 || tabs.Children.length <= which)
                    return false;
                return tabs.Children[which];
            }
            if (typeof which == "string") {
                $(tabs.Children).each(function () {
                    if (this.Title == which) {
                        exists = true;
                        return;
                    }
                });

                return exists;
            }

            return false;
        },
        GetTabIndex: function (title) {
            ///<summary>
            /// 获取指定名称的选项卡的对应索引值
            ///</summary>
            ///<param name="title" type="string">选项卡的名称</param>
            var tabs = this, options = this.Options;

            var index = -1;
            $(tabs.Children).each(function (idx) {
                if (this.Title == title) {
                    index = idx;
                    return;
                }
            });

            return index;
        },
        GetSelectedTab: function () {
            ///<summary>
            /// 获取当前被选中的选项卡
            ///</summary>
            var tabs = this, options = this.Options;
            var active = $("li.active", tabs.NavBar);
            if (active.length > 0) {
                var idx = active.attr("idx");
                return tabs.Children[idx];
            }
            else
                return;
        },
        Close: function (which) {
            ///<summary>
            /// 关闭一个选项卡面板
            ///</summary>
            ///<param name="which" type="which">表示哪个选项卡将被关闭,参数指向选项卡标题或索引。</param>
            var tabs = this, options = this.Options;

            if (!tabs.Exists(which))
                return;

            var index = (typeof which == "number") ? which : tabs.GetTabIndex(which);
            var isClose = true;
            var tab = tabs.NavBar.children().eq(index);

            if (tabs.HasBind("BeforeClose"))
                isClose = tabs.Trigger("BeforeClose", [tab, index]);
            if (!isClose)
                return;

            tabs.NavBarWidth -= tab.outerWidth(true);
            tab.remove();
            tabs.Contents.children().eq(index).remove();
            tabs.Children.splice(index, 1);
            tabs._RefreshIndex();
            tabs._ToggleButton();

            if (tabs.HasBind("Close"))
                tabs.Trigger("Close", []);
        },
        _RefreshIndex: function () {
            var tabs = this, options = this.Options;
            $(tabs.Children).each(function (idx) {
                var navItem = tabs.NavBar.children().eq(idx);
                var id = tabs._CreateTabItemId(idx);
                navItem.attr("idx", idx);
                $("a", navItem).attr("idx", idx).attr("href", "#" + id);
                tabs.Contents.children().eq(idx).attr("idx", idx).attr("id", id);
            });
        }
    });
})(jQuery);