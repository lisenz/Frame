/*
* FrameUI v2.1.0
*
* 作者：zlx
* 日期：2015.05.26
* 说明：Pagination 分页控件
*
* Copyright 2015 zlx
*
*/
(function ($) {
    $.fn.Pagination = function (options) {
        return $.Frame.Do.call(this, "Pagination", arguments);
    };

    $.Methods.Pagination = $.Methods.Pagination || {};

    $.Defaults.Pagination = {
        Total: 0,              // 总记录数，当初始化创建分页时必须设置
        PageSize: 10,          // 每页显示的记录数
        PageNumber: 1,         // 当分页创建完毕时显示当前页码
        Buttons: null,         // 自定义功能按钮组,每个按钮对象包含3个属性:Icon,Title,Color,OnClick/Separator
        OnSelectPage: false    // 当用户进行翻页时触发，回调函数包含2个参数:PageNumber,PageSize
    };

    $.Frame.Controls.Pagination = function (element, options) {
        $.Frame.Controls.Pagination.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Pagination.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Pagination";
        },
        _IdPrev: function () {
            return "PaginationFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Pagination;
        },
        _PreRender: function () {
            var pagination = this;

            // TODO：这里为了防止页面标签元素重复设置样式
            if (!$(pagination.Element).hasClass("frame-pagination")) {
                $(pagination.Element).addClass("frame-pagination");
            }

            pagination.PageTotal = 0;   // 数据表格的总页数
            pagination.CurrentPage = 0; // 记录当前页的页码
        },
        _Render: function () {
            var pagination = this, options = this.Options;
            pagination.Container = $("<div></div>").addClass("frame-pagination-control").appendTo(pagination.Element);
            pagination.Table = $("<table><tbody><tr></tr></tbody></table>").css("width", "100%")
                .addClass("frame-pagination-table").appendTo(pagination.Container);
            pagination.ToolBar = pagination._CreateToolBar();
            pagination.PageBar = pagination._CreatePageBar();
            pagination.InfPart = pagination._CreateInfPart();
            $("tr", pagination.Table).append(pagination.ToolBar).append(pagination.PageBar).append(pagination.InfPart);

            pagination.Set(options);
        },
        _CreateToolBar: function () {
            ///<summary>
            /// 创建分页控件中的工具栏
            ///</summary>
            var cell = $("<td></td>").attr("align", "left");
            var toolbar = $("<table><tbody><tr></tr></tbody></table>").addClass("frame-pagination-table").addClass("toolbar")
                                                                      .attr("border", "0")
                                                                      .attr("cellpadding", "0")
                                                                      .attr("cellspacing", "0");
            cell.append(toolbar);
            return cell;
        },
        _CreatePageBar: function () {
            ///<summary>
            /// 创建分页控件的翻页区域
            ///</summary>
            var pagination = this;
            var cell = $("<td></td>").attr("align", "center");
            var pagebar = $("<table><tbody><tr></tr></tbody></table>").addClass("frame-pagination-table").addClass("pagebar")
                                                                      .attr("border", "0")
                                                                      .attr("cellpadding", "0")
                                                                      .attr("cellspacing", "0");
            pagination.FirstButton = pagination._CreatePageButton("icon-double-angle-left");
            pagination.PrevButton = pagination._CreatePageButton("icon-angle-left");
            var input = pagination._CreateInputOnPage();
            pagination.NextButton = pagination._CreatePageButton("icon-angle-right");
            pagination.LastButton = pagination._CreatePageButton("icon-double-angle-right");

            $("tr", pagebar).append(pagination.FirstButton).append(pagination.PrevButton)
                            .append(pagination._CreateSeparatorOnPage())
                            .append(input)
                            .append(pagination._CreateSeparatorOnPage())
                            .append(pagination.NextButton).append(pagination.LastButton);
            cell.append(pagebar);

            return cell;
        },
        _CreatePageButton: function (icon) {
            ///<summary>
            /// 创建分页控件翻页区域的上一页按钮及绑定相关事件
            ///</summary>
            ///<param name="icon" type="string">按钮图标样式:[icon-double-angle-left,icon-angle-left,icon-angle-right,icon-double-angle-right].</param>
            var pagination = this, options = this.Options;
            var cell = $("<td></td>").addClass("frame-pagination-button").addClass("frame-pagination-state-disabled");
            var angle = $("<span></span>").addClass("frame-pagination-icon").addClass(icon)
                                         .addClass("bigger-140");

            cell.append(angle).on("click", function () {
                if ($(this).hasClass("frame-pagination-state-disabled"))
                    return;

                if ($("span", this).hasClass("icon-double-angle-left")) {
                    pagination._First();
                }
                if ($("span", this).hasClass("icon-angle-left")) {
                    pagination._Prev();
                }
                if ($("span", this).hasClass("icon-angle-right")) {
                    pagination._Next();
                }
                if ($("span", this).hasClass("icon-double-angle-right")) {
                    pagination._Last();
                }

            });

            return cell;
        },
        _CreateSeparatorOnPage: function () {
            ///<summary>
            /// 创建分页控件翻页区域的分割线
            ///</summary>
            var cell = $("<td></td>").addClass("frame-pagination-button").addClass("frame-pagination-state-disabled")
                                     .addClass("separator");
            var separator = $("<span></span>").addClass("frame-pagination-separator");
            cell.append(separator);

            return cell;
        },
        _CreateInputOnPage: function () {
            ///<summary>
            /// 创建分页控件翻页区域的页码输入框
            ///</summary>
            var pagination = this, options = this.Options;
            var cell = $("<td></td>").addClass("input");
            var input = $("<input />").addClass("frame-pagination-input")
                                      .attr("size", "2")
                                      .attr("maxlength", "7")
                                      .attr("type", "text").val(0).on("blur.TextBox", function () {
                                          var reg = new RegExp("^[1-9]*$");
                                          var v = $(this).val();
                                          if (!reg.test(v)) {
                                              v = pagination.CurrentPage;
                                          }
                                          pagination._GoTo(v);
                                          return false;
                                      });
            cell.append("第 ").append(input).append(" 页 &nbsp;总页数：").append("<span>0</span>");
            return cell;
        },
        _GoTo: function (page) {
            var pagination = this, options = this.Options;

            // 只有当页数>0时(即有数据),才会生效
            if (pagination.PageTotal > 0) {
                pagination.CurrentPage = page;
                pagination.Set({ PageNumber: pagination.CurrentPage });
            }
        },
        _CreateInfPart: function () {
            ///<summary>
            /// 创建分页文本显示信息区域
            ///</summary>
            var cell = $("<td></td>").attr("align", "right");
            var info = $("<div></div>").addClass("frame-pagination-info").css("text-align", "right").append("显示记录 1 - 10 &nbsp;总记录数：23");
            cell.append(info);

            return cell;
        },
        _SetTotal: function (total) {
            ///<summary>
            /// 设置表格的总记录数,计算数据的页数
            ///</summary>
            ///<param name="total" type="number">数据的总记录数.</param>
            var pagination = this, options = this.Options;
            if (total > 0) {
                var size = Math.floor((total + options.PageSize - 1) / options.PageSize); // 当前记录数可以被分为几页
                pagination.PageTotal = size;
                $(".input span", pagination.PageBar).text(size); // Page * of 后的页数 
            }
        },
        _SetPageNumber: function (num) {
            ///<summary>
            /// 设置更新数据表格当前页码
            ///</summary>
            ///<param name="num" type="number">当前页码.</param>
            var pagination = this, options = this.Options;
            var startNum = 0;
            var endNum = 0;

            pagination.CurrentPage = 0;
            if (options.Total > 0) {
                pagination.CurrentPage = num;
                startNum = ((num - 1) * options.PageSize + 1);
                endNum = num * options.PageSize;
                endNum = endNum > options.Total ? options.Total : endNum;
            }
            $(".input input.frame-pagination-input", pagination.PageBar).val(pagination.CurrentPage);
            $(".frame-pagination-info", pagination.InfPart).empty().append("显示记录 " + startNum + " - " + endNum + " &nbsp;总记录数：" + options.Total);

            pagination._PageButtonChange();

            if (pagination.HasBind("SelectPage") && pagination.CurrentPage > 0) {
                pagination.Trigger("SelectPage", [pagination.CurrentPage, options.PageSize]);
            }
        },
        _PageButtonChange: function () {
            ///<summary>
            /// 设置翻页按钮及页码输入文本框的禁用状态
            ///</summary>
            var pagination = this, options = this.Options;

            if (pagination.CurrentPage == 1 && pagination.CurrentPage == pagination.PageTotal) {
                // 当前位于首页，且页数只有一页，4个翻页按钮都禁用
                pagination._DisablePageButton("FirstButton");
                pagination._DisablePageButton("PrevButton");
                pagination._DisablePageButton("NextButton");
                pagination._DisablePageButton("LastButton");
            }
            if (pagination.CurrentPage == 1 && pagination.CurrentPage < pagination.PageTotal) {
                // 当前位于首页，且页数不止一页，下一页及尾页翻页按钮可用
                pagination._DisablePageButton("FirstButton");
                pagination._DisablePageButton("PrevButton");
                pagination._EnablePageButton("NextButton");
                pagination._EnablePageButton("LastButton");
            }
            if (pagination.CurrentPage > 1 && pagination.CurrentPage < pagination.PageTotal) {
                // 当前位于中间区间，4个翻页按钮可用
                pagination._EnablePageButton("FirstButton");
                pagination._EnablePageButton("PrevButton");
                pagination._EnablePageButton("NextButton");
                pagination._EnablePageButton("LastButton");
            }
            if (pagination.CurrentPage > 1 && pagination.CurrentPage == pagination.PageTotal) {
                // 当前位于尾页，下一页及尾页翻页按钮禁用
                pagination._EnablePageButton("FirstButton");
                pagination._EnablePageButton("PrevButton");
                pagination._DisablePageButton("NextButton");
                pagination._DisablePageButton("LastButton");
            }

        },
        _EnablePageButton: function (key) {
            var pagination = this;
            if (pagination[key].hasClass("frame-pagination-state-disabled"))
                pagination[key].removeClass("frame-pagination-state-disabled");
        },
        _DisablePageButton: function (key) {
            var pagination = this;
            if (!pagination[key].hasClass("frame-pagination-state-disabled"))
                pagination[key].addClass("frame-pagination-state-disabled");
        },
        _First: function () {
            var pagination = this;
            pagination.CurrentPage = 1;
            pagination.Set({ PageNumber: 1 });
        },
        _Prev: function () {
            var pagination = this;
            if (pagination.CurrentPage > 1)
                pagination.CurrentPage--;
            pagination.Set({ PageNumber: pagination.CurrentPage });
        },
        _Next: function () {
            var pagination = this;
            if (pagination.CurrentPage < pagination.PageTotal)
                pagination.CurrentPage++;
            pagination.Set({ PageNumber: pagination.CurrentPage });
        },
        _Last: function () {
            var pagination = this;
            pagination.CurrentPage = pagination.PageTotal;
            pagination.Set({ PageNumber: pagination.PageTotal });
        },
        _SetButtons: function (buttons) {
            var pagination = this, options = this.Options;
            if (buttons && buttons.length > 0) {
                $(buttons).each(function (i) {
                    var cell = $("<td></td>").addClass("frame-pagination-button");
                    if (!this.Separator) {
                        var button = $("<div></div>").attr("idx", i).addClass("frame-pagination-div").appendTo(cell)
                        .on("click", function () {
                            var idx = $(this).attr("idx");
                            if (buttons[idx].Click) {
                                buttons[idx].Click.call(this, buttons[idx].Title);
                            }
                            return false;
                        });
                        this.Color = this.Color || "grey";
                        var icon = $("<span></span>").addClass("frame-pagination-icon").addClass(this.Icon).addClass(this.Color)
                                                     .attr("tip", this.Title).appendTo(button);
                        this.Title && $(icon).Tip({ Placement: "auto", Mode: "inverse" });
                    }
                    else {
                        cell.addClass("separator").append($("<span></span>").addClass("frame-pagination-separator"));
                    }
                    $(".toolbar tr", pagination.ToolBar).append(cell);
                });
            }
        },
        Refresh: function (total, pagenumber) {
            ///<summary>
            /// 根据总记录数和当前页码,刷新分页的页数
            ///</summary>
            ///<param name="total" type="number">数据的总记录数。</param>
            ///<param name="pagenumber" type="number">当前设置的页码。</param>
            var pagination = this, options = this.Options;
            pagination.Set({ Total: total, PageNumber: pagenumber });
        }
    });

})(jQuery);