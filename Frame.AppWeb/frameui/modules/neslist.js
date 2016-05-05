/**
* jQuery FrameUI v2.2.0
* 
* 作者：zlx
* 日期：2016.03.02
* 说明：NesList 控件。
*/
(function ($) {
    $.fn.NesList = function (options) {
        return $.Frame.Do.call(this, "NesList", arguments);
    };

    $.Methods.NesList = $.Methods.NesList || {};

    $.Defaults.NesList = {
        Width: "auto",
        Height: "auto",
        Items: [],         // Item:{Text,Commands:[Icon,Color,Tip,OnClick],Childs:{...}}
        OnItemCollapse: null, //{IsCollapse,Render,Element}
        OnCommand: null
    };

    $.Frame.Controls.NesList = function (element, options) {
        $.Frame.Controls.NesList.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.NesList.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "NesList";
        },
        _IdPrev: function () {
            return "NesListFor";
        },
        _ExtendMethods: function () {
            return $.Methods.NesList;
        },
        _PreRender: function () {
            var neslist = this;

            neslist.Children = [];
            if (!$(neslist.Element).hasClass("frame-neslist")) {
                $(neslist.Element).addClass("frame-neslist");
            }
        },
        _Render: function () {
            var neslist = this, options = this.Options;

            neslist.Container = $("<ol></ol>").addClass("frame-neslist-list").appendTo(neslist.Element);

            neslist.Set(options);
        },
        _SetWidth: function (width) {
            var neslist, options = this.Options;
            if (width != "auto") {
                $(neslist.Element).width(width);
            }
        },
        _SetHeight: function (height) {
            var neslist, options = this.Options;
            if (height != "auto") {
                $(neslist.Element).height(height);
            }
        },
        _SetItems: function (items) {
            var neslist = this;

            if (items && items.length > 0) {
                neslist.Children = items;
                $(neslist.Children).each(function (i, item) {
                    neslist.Children[i].__Priority = 1;   //设置该节点的层级
                    var itemElement = $("<li></li>").addClass("frame-neslist-item").appendTo(neslist.Container);
                    neslist._AppendItemForeach(1, i, itemElement, item);
                });
            }
        },
        _AppendItemForeach: function (level, index, ele, item, parent) {
            var neslist = this;

            var lvid = parent ? parent.__Lvid + level.toString() + index.toString() : level.toString() + index.toString();
            item.__Lvid = lvid;
            ele.attr("priority", level).attr("lv-id", lvid);
            var contentElement = $("<div></div>").addClass("frame-neslist-item-content").append($("<span></span>").text(item.Text)).appendTo(ele);
            if (item.Commands && item.Commands.length > 0) {
                var commands = $("<div></div>").addClass("pull-right").addClass("action-buttons").appendTo(contentElement);
                $(item.Commands).each(function (i, command) {
                    $('<a href="#"></a>').addClass(command.Color || "blue")
                                         .attr("idx", i)
                                         .append($("<i></i>").addClass(command.Icon).addClass("bigger-130"))
                                         .appendTo(commands)
                                         .click(function () {
                                             if (neslist.HasBind("Command")) {
                                                 neslist.Trigger("Command", [this, { Idx: index, Self: item.Commands[i], Priority: level, For: lvid, Parent: ele}]);
                                             }
                                         });
                });
            }

            if (item.Childs && item.Childs.length > 0) {
                ele.addClass("frame-neslist-collapsed");
                ele.prepend($('<button type="button">Collapse</button>').click(function () {
                    var collapse = $(this).hasClass("frame-neslist-item-collapse");
                    if (!collapse) {
                        $(this).addClass("frame-neslist-item-collapse");
                        $(this).parent().removeClass("frame-neslist-collapsed");
                    } else {
                        $(this).removeClass("frame-neslist-item-collapse");
                        $(this).parent().addClass("frame-neslist-collapsed");
                    }

                    if (neslist.HasBind("ItemCollapse")) {
                        neslist.Trigger("ItemCollapse", [{ IsCollapse: collapse, Render: neslist, Element: this}]);
                    }

                }));
                var container = $("<ol></ol>").addClass("frame-neslist-list").appendTo(ele);
                $(item.Childs).each(function (j, child) {
                    item.Childs[j].__Priority = level + 1;  //设置该节点的层级
                    var itemElement = $("<li></li>").addClass("frame-neslist-item").appendTo(container);
                    neslist._AppendItemForeach(level + 1, j, itemElement, child, item);
                });
            }
        },
        Load: function (items) {
            var neslist = this;
            neslist._SetItems(items);
        },
        Append: function (lvid, item) {
            var neslist = this;

            if (neslist.Children.length > 0) {
                neslist._AppendItemForLvid(lvid, neslist.Children, item);
            }
        },
        _AppendItemForLvid: function (lvid, items, item) {
            var neslist = this;
            $(items).each(function (i, child) {
                if (child.__Lvid == lvid) {
                    var priority = child.__Priority + 1;
                    item.__Priority = priority;
                    child.Childs.push(item);


                    var container = $("[lv-id=" + lvid + "]", neslist.Container);
                    var itemList = $(".frame-neslist-list", container);
                    if (itemList.length == 0) {
                        itemList = $("<ol></ol>").addClass("frame-neslist-list").appendTo(container);
                    }
                    var index = child.Childs.length - 1;
                    var itemElement = $("<li></li>").addClass("frame-neslist-item").appendTo(itemList);
                    if (!container.hasClass("frame-neslist-collapsed") && $("button", container).first().length == 0) {
                        container.addClass("frame-neslist-collapsed");
                        container.prepend($('<button type="button">Collapse</button>').click(function () {
                            var collapse = $(this).hasClass("frame-neslist-item-collapse");
                            if (!collapse) {
                                $(this).addClass("frame-neslist-item-collapse");
                                $(this).parent().removeClass("frame-neslist-collapsed");
                            } else {
                                $(this).removeClass("frame-neslist-item-collapse");
                                $(this).parent().addClass("frame-neslist-collapsed");
                            }

                            if (neslist.HasBind("ItemCollapse")) {
                                neslist.Trigger("ItemCollapse", [{ IsCollapse: collapse, Render: neslist, Element: this}]);
                            }

                        }));
                    }

                    neslist._AppendItemForeach(priority, index, itemElement, item, child);

                    return;
                }
                else
                    neslist._AppendItemForLvid(lvid, child.Childs, item);
            });
        }
    });

})(jQuery);