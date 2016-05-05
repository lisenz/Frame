/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.05.25
* 说明：Grid 表格控件。
*/
(function ($) {
    $.fn.Grid = function (options) {
        return $.Frame.Do.call(this, "Grid", arguments);
    };

    $.Methods.Grid = $.Methods.Grid || {};

    $.Defaults.Grid = {
        CanShowPager: true,
        CanShowCheckBox: false,
        CanShowRowNumber: false,
        CanShowLoading: false,
        Message: "正在加载数据,请稍候...",
        Page: 1,    // 当前页码
        PageSize: 10, // 每页显示的记录数
        FrozenColumns: [],     //column{Title,Field,Width,Align,Hidden,Formatter,Styler}
        Columns: [],           //column{Title,Field,Width,Align,Hidden,Formatter,Styler}
        DataSource: null,
        Method: "POST",
        DataType: "json",
        Params: {},
        Url: false,
        Striped: true,
        CanMultiple: false,
        PageButtons: [],
        Width: "auto",
        Height: "auto",
        OnSuccess: null,                       //成功获取服务器数据的事件
        OnSelectRow: null,                    //选择行事件
        OnUnSelectRow: null,                   //取消选择行事件
        OnBeforeCheckRow: null,                 //选择前事件，可以通过return false阻止操作(复选框)
        OnCheckRow: null,                    //选择事件(复选框) 
        OnBeforeCheckAllRow: null,              //选择前事件，可以通过return false阻止操作(复选框 全选/全不选)
        OnCheckAllRow: null,                    //选择事件(复选框 全选/全不选)
        OnBeforeShowData: null,                  //显示数据前事件，可以通过reutrn false阻止操作
        OnAfterShowData: null,                 //显示完数据事件
        OnError: null,                         //错误事件
        OnLoading: null,                        //加载时函数
        OnLoaded: null,                          //加载完函数
        OnComplete: null,                       //请求完成后回调函数 (请求成功或失败之后均调用)。
        OnExtractor: null
    };

    $.Frame.Controls.Grid = function (element, options) {
        $.Frame.Controls.Grid.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Grid.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Grid";
        },
        _IdPrev: function () {
            return "GridFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Grid;
        },
        _PreRender: function () {
            var grid = this, options = this.Options;

            // TODO：这里为了防止页面标签元素重复设置样式
            if (!$(grid.Element).hasClass("frame-grid")) {
                $(grid.Element).addClass("frame-grid");
            }

            grid.DataSource = [];
            grid.Selections = [];
        },
        _Render: function () {
            var grid = this, options = this.Options;

            grid.ViewContainer = $("<div></div>").addClass("frame-grid-view").appendTo(grid.Element);

            // 冻结部分
            grid._CreateFreezedContainer();

            // 常规部分
            grid._CreateGroovyContainer();

            grid.Set(options);
            grid.Resize();
        },
        _CreateFreezedContainer: function () {
            var grid = this, options = this.Options;

            grid.FreezeViewContainer = $("<div></div>").addClass("frame-grid-freeze-view").appendTo(grid.ViewContainer);
            grid.FreezeHeadContainer = $("<div></div>").addClass("frame-grid-freeze-hdiv").appendTo(grid.FreezeViewContainer);
            grid.FreezeHeadBox = $("<div></div>").addClass("frame-grid-freeze-hbox").appendTo(grid.FreezeHeadContainer);
            grid.FreezeHeadTable = $("<table></table>").addClass("frame-grid-freeze-htable").attr("cellpadding", "0")
                                                       .attr("cellspacing", "0")
                                                       .attr("border", "0")
                                                       .append($("<thead></thead>")
                                                                .append($("<tr></tr>")
                                                                    .addClass("frame-grid-freeze-labels")))
                                                       .appendTo(grid.FreezeHeadBox);
            grid.FreezeContentContainer = $("<div></div>").addClass("frame-grid-freeze-bdiv").appendTo(grid.FreezeViewContainer);
            grid.FreezeContentBox = $("<div></div>").addClass("frame-grid-freeze-bbox").appendTo(grid.FreezeContentContainer);
            grid.FreezeTable = $("<table></table>").addClass("frame-grid-freeze-btable").attr("cellpadding", "0")
                                                       .attr("cellspacing", "0")
                                                       .attr("border", "0")
                                                       .append("<tbody></tbody>").appendTo(grid.FreezeContentBox);
        },
        _CreateGroovyContainer: function () {
            var grid = this, options = this.Options;

            grid.GroovyViewContainer = $("<div></div>").addClass("frame-grid-groovy-view").appendTo(grid.ViewContainer);
            grid.GroovyHeadContainer = $("<div></div>").addClass("frame-grid-groovy-hdiv").appendTo(grid.GroovyViewContainer);
            grid.GroovyHeadBox = $("<div></div>").addClass("frame-grid-groovy-hbox").appendTo(grid.GroovyHeadContainer);
            grid.GroovyHeadTable = $("<table></table>").addClass("frame-grid-groovy-htable").attr("cellpadding", "0")
                                                       .attr("cellspacing", "0")
                                                       .attr("border", "0")
                                                       .append($("<thead></thead>")
                                                                .append($("<tr></tr>")
                                                                    .addClass("frame-grid-groovy-labels")))
                                                       .appendTo(grid.GroovyHeadBox);
            grid.GroovyContentContainer = $("<div></div>").addClass("frame-grid-groovy-bdiv").appendTo(grid.GroovyViewContainer);
            grid.GroovyContentBox = $("<div></div>").addClass("frame-grid-groovy-bbox").appendTo(grid.GroovyContentContainer);
            grid.GroovyTable = $("<table></table>").addClass("frame-grid-groovy-btable").attr("cellpadding", "0")
                                                       .attr("cellspacing", "0")
                                                       .attr("border", "0")
                                                       .append("<tbody></tbody>").appendTo(grid.GroovyContentBox);
        },
        _SetCanShowPager: function (showPager) {
            ///<summary>
            /// 是否使用分页
            ///</summary>
            ///<param name="showPager" type="bool">设置一个值，该值标识是否使用分页控件。</param>
            var grid = this, options = this.Options;

            if (showPager) {
                grid.Pagination = $("<div></div>").addClass("frame-pagination").appendTo(grid.Element);
                $.extend(options.Params, { page: options.Page, pagesize: options.PageSize });
                grid.Pagination = $(grid.Pagination).Pagination({ PageSize: options.PageSize, Buttons: options.PageButtons,
                    OnSelectPage: function (page, size) {
                        // page:当前页码,size:每页行数
                        options.Page = page;
                        $.extend(options.Params, { page: page, pagesize: size });
                        grid.Load({});
                    }
                });
            }
        },
        _SetCanShowLoading: function (loading) {
            var grid = this, options = this.Options;
            if (loading) {
                var overlay = $("<div></div>").addClass("frame-grid-overlay");
                var l = $("<div></div>").addClass("loading").text(options.Message);
                grid.ViewContainer.prepend(l).prepend(overlay);
            }
        },
        _ShowLoading: function () {
            var grid = this, options = this.Options;
            $(".frame-grid-overlay", grid.ViewContainer).show();
            $(".loading", grid.ViewContainer).show();
        },
        _CloseLoading: function () {
            var grid = this, options = this.Options;
            $(".frame-grid-overlay", grid.ViewContainer).hide();
            $(".loading", grid.ViewContainer).hide();
        },
        _SetUrl: function (url) {
            /// <summary>
            /// 请求远程数据并加载到表格
            ///</summary>
            ///<param name="url" type="string">请求的URL</param>
            var grid = this, options = this.Options;
            if (url) {
                options.Url = url;
                grid.Load(options.Params);
            }
        },
        _CreateHeaderTag: function () {
            ///<summary>
            /// 创建一个数据表头列
            ///</summary>
            return $("<th></th>").addClass("frame-grid-table-th-column").addClass("frame-grid-table-th-ltr");
        },
        _CreateHeaderColumnTag: function (text) {
            return $("<div></div>").addClass("frame-grid-sortable").text(text);
        },
        _CompareColumnWidth: function (original, compare) {
            /// <summary>
            /// 比较两个宽度值.
            ///</summary>
            ///<param name="original" type="number">原宽度</param>
            ///<param name="compare" type="number">要进行比较的宽度</param>
            if (original && original > compare)
                return original;
            else
                return compare;
        },
        _CreateCheckBoxColumnOnHead: function () {
            ///<summary>
            /// 创建冻结数据表头的复选框列
            ///</summary>
            var grid = this, options = this.Options;

            if (options.CanShowCheckBox) {
                var chk_header_column = grid._CreateHeaderTag();
                var chk_header_div = $("<div></div>").addClass("frame-grid-sortable").addClass("frame-grid-sortable-chk");
                chk_header_column.append(chk_header_div);
                $(".frame-grid-freeze-labels", grid.FreezeHeadTable).append(chk_header_column);

                if (options.CanMultiple) {
                    var chk = $("<input type='checkbox' />").attr("id", grid.Id + "_chk_all").appendTo(chk_header_div);
                    chk.CheckBox({
                        OnChange: function (checked) {
                            if (checked) {
                                grid.SelectAll();
                            }
                            else {
                                grid.UnSelectAll();
                            }
                        }
                    });
                }
            }
        },
        _CreateRowNumberColumnOnHead: function () {
            ///<summary>
            /// 创建冻结数据表头的行号列
            ///</summary>
            var grid = this, options = this.Options;

            if (options.CanShowRowNumber) {
                var rownum_column = grid._CreateHeaderTag();
                var rownum_div = $("<div></div>").addClass("frame-grid-sortable").addClass("frame-grid-sortable-num");
                rownum_column.append(rownum_div);
                $(".frame-grid-freeze-labels", grid.FreezeHeadTable).append(rownum_column);
            }
        },
        _CreateColumnOnHead: function (column, trow) {
            ///<summary>
            /// 创建数据表头列
            ///</summary>
            ///<param name="trow" type="object">数据行对象。</param>
            var grid = this, options = this.Options;

            var th_column = grid._CreateHeaderTag();
            trow.append(th_column);
            var col = grid._CreateHeaderColumnTag(column.Title);
            th_column.append(col);
            if (options.FitColumns)
                column.Width = grid._CompareColumnWidth(column.Width, col.width());
            col.width(column.Width);
            if (column.Align)
                th_column.css("text-align", column.Align);
        },
        _SetColumns: function (columns) {
            ///<summary>
            /// 创建常规数据表头
            ///</summary>
            ///<param name="columns" type="array">常规表头的列对象集合。</param>
            var grid = this, options = this.Options;

            if (columns && columns.length > 0) {
                $(columns).each(function () {
                    if (!this.Hidden) {
                        grid._CreateColumnOnHead(this, $(".frame-grid-groovy-labels", grid.GroovyHeadTable));
                    }
                });
            }

            if (!options.CanShowCheckBox && !options.CanShowRowNumber && options.FrozenColumns.length == 0) {
                grid.GroovyHeadBox.addClass("frame-grid-groovy-hbox-ltr");
                grid.GroovyContentContainer.addClass("frame-grid-groovy-bdiv-ltr");
            }
        },
        _SetFrozenColumns: function (columns) {
            ///<summary>
            /// 创建冻结数据表头
            ///</summary>
            ///<param name="columns" type="array">冻结表头的列对象集合。</param>
            var grid = this, options = this.Options;

            grid._CreateCheckBoxColumnOnHead();
            grid._CreateRowNumberColumnOnHead();

            if (columns && columns.length > 0) {
                $(columns).each(function () {
                    if (!this.Hidden) {
                        grid._CreateColumnOnHead(this, $(".frame-grid-freeze-labels", grid.FreezeHeadTable));
                    }
                });
            }

            if (!options.CanShowCheckBox && !options.CanShowRowNumber && columns.length == 0) {
                grid.FreezeViewContainer.hide();
            }
        },
        _SetWidth: function (width) {
            var grid = this, options = this.Options;
            if (width != "auto") {
                $(grid.Element).width(width);
                grid.ViewContainer.width(width);
            } else {
                var parentWidth = $(grid.Element).parent().width();
                grid.ViewContainer.width(parentWidth);
            }
        },
        _SetHeight: function (height) {
            var grid = this, options = this.Options;
            if (height != "auto") {
                $(grid.Element).height(height);
                grid.ViewContainer.height(height);
            } else {
                var parentHeight = $(grid.Element).parent().height();
                var pageHeight = options.CanShowPager ? 55 : 0;
                $(grid.Element).height(parentHeight);
                grid.ViewContainer.height(parentHeight - pageHeight);
            }
        },
        _SetDataSource: function (data) {
            var grid = this, options = this.Options;

            if (data) {
                if (options.CanShowPager)
                    grid.Pagination.Refresh(data.Total, data.Total == 0 ? 0 : options.Page);

                if (data.Rows.length > 0) {
                    grid.DataSource = data.Rows;

                    $(data.Rows).each(function (idx) {
                        grid._CreateFreezeRow(idx, this);
                        grid._CreateGroovyRow(idx, this);
                    });

                    if (options.Striped) {
                        $("tr:odd", grid.FreezeTable).addClass("frame-grid-priority-secondary");
                        $("tr:odd", grid.GroovyTable).addClass("frame-grid-priority-secondary");
                    }

                    grid._BindingRowStateOnHover(grid.FreezeTable, grid.GroovyTable);
                    grid._BindingRowStateOnHover(grid.GroovyTable, grid.FreezeTable);
                }
            }
        },
        _BindingRowStateOnHover: function (bindingTable, toggleTable) {
            ///<summary>
            /// 对数据行元素绑定mouseover,mouseout,click事件,实现鼠标hover样式及行单击选中样式
            ///</summary>
            var grid = this, options = this.Options;
            $("tr", bindingTable).on("mouseover mouseout click", function (event) {
                var idx = $(this).attr("r_idx");
                var row_toggle = $("tr.frame-grid-row[r_idx=" + idx + "]", toggleTable);
                switch (event.type) {
                    case "mouseover": grid._RowOnMouseOver(idx, this, row_toggle); break;
                    case "mouseout": grid._RowOnMouseOut(idx, this, row_toggle); break;
                    case "click": grid._RowOnClick(idx, this, row_toggle); break;
                }

                return false;
            });
        },
        _RowOnMouseOver: function (idx, row_freeze, row_groovy) {
            var grid = this, options = this.Options;
            $(row_freeze).addClass("frame-grid-state-hover");
            $(row_groovy).addClass("frame-grid-state-hover");
        },
        _RowOnMouseOut: function (idx, row_freeze, row_groovy) {
            var grid = this, options = this.Options;
            $(row_freeze).removeClass("frame-grid-state-hover");
            $(row_groovy).removeClass("frame-grid-state-hover");
        },
        _RowOnClick: function (idx, row_freeze, row_groovy) {
            var grid = this, options = this.Options;
            if (!options.CanMultiple) {
                // 单选模式
                grid._CancelOtherwiseRow(idx);
            }
            $(row_freeze).toggleClass("frame-grid-state-highlight");
            $(row_groovy).toggleClass("frame-grid-state-highlight");
            grid._ToggleRecord(idx);

            /* TODO: 这里作用除了切换复选框勾选状态,当绑定了BeforeCheckRow事件且通过return false阻止操作.
            行元素的高亮将会重新被取消 */
            if (!grid._ToggleCheckBox(idx)) {
                $(row_freeze).toggleClass("frame-grid-state-highlight");
                $(row_groovy).toggleClass("frame-grid-state-highlight");
                grid._ToggleRecord(idx);
            }
        },
        _ToggleCheckBox: function (idx) {
            ///<summary>
            /// 复选框勾选状态切换
            ///</summary>
            ///<param name="idx" type="number">指定行的复选框索引。</param>
            var grid = this, options = this.Options;

            if (options.CanShowCheckBox) {
                var chkState = $("#" + grid.Id + "_row_chk_" + idx).Frame("Options", "Checked");
                if (chkState)
                    $("#" + grid.Id + "_row_chk_" + idx).Frame().Checked(false);
                else {
                    var hook = true;
                    if (grid.HasBind("BeforeCheckRow"))
                        hook = grid.Trigger("BeforeCheckRow", [idx]); // idx为行索引
                    if (hook) {
                        $("#" + grid.Id + "_row_chk_" + idx).Frame().Checked(true);
                    }
                    return hook;
                }
            }

            return true;
        },
        _CancelOtherwiseRow: function (idx) {
            ///<summary>
            /// 若当前单击的行与被选中的行不相同时,取消被选中行的所有状态
            ///</summary>
            ///<param name="idx" type="number">指定行的复选框索引。</param>
            var grid = this, options = this.Options;
            var row_highlight_freeze = $("tr.frame-grid-state-highlight", grid.FreezeTable);
            var row_highlight_groovy = $("tr.frame-grid-state-highlight", grid.GroovyTable);

            /* 这里判断之前是否有被选择的行元素,若有,则用来与当前单击的行索引进行判断是否为同一行,
            如果是同一个行元素,则跳过;如果不是同一个行元素,则取消行元素高亮 */
            if (row_highlight_groovy.length > 0) {
                var select_idx = row_highlight_groovy.attr("r_idx");
                if (select_idx != idx) {
                    $(row_highlight_freeze).removeClass("frame-grid-state-highlight");
                    $(row_highlight_groovy).removeClass("frame-grid-state-highlight");
                    grid._ToggleCheckBox(select_idx);
                    grid._RemoveRecord(select_idx);
                }
            }

        },
        _CreateFreezeRow: function (idx, data) {
            var grid = this, options = this.Options;
            var row = $("<tr></tr>").attr("r_idx", idx).addClass("frame-grid-row").addClass("frame-grid-row-ltr");
            $("tbody", grid.FreezeTable).append(row);

            grid._CreateCheckBoxColumnOnRow(idx);
            grid._CreateRowNumberColumnOnRow(idx);

            if (options.FrozenColumns.length > 0) {
                $(options.FrozenColumns).each(function () {
                    grid._PackCellFetchRow(idx, grid.FreezeTable, this, data);
                });
            }
        },
        _CreateCheckBoxColumnOnRow: function (idx) {
            var grid = this, options = this.Options;
            if (options.CanShowCheckBox) {
                var chk_column = $("<td></td>");
                var chk_column_div = $("<div></div>").addClass("frame-grid-sortable").addClass("frame-grid-sortable-chk");
                chk_column.append(chk_column_div);
                $("tr.frame-grid-row[r_idx=" + idx + "]", grid.FreezeTable).append(chk_column);

                var chk = $("<input type='checkbox' />").attr("id", grid.Id + "_row_chk_" + idx).attr("idx", idx).appendTo(chk_column_div);
                chk.CheckBox({
                    OnChange: function (state) {
                        if (state && grid.HasBind("CheckRow")) {
                            var index = $(this.Element).attr("idx");
                            var row = grid._FindRecordData(index);
                            grid.Trigger("CheckRow", [index, row]); //index:行索引,row:数据行记录对象
                        }
                    }
                });
            }
        },
        _CreateRowNumberColumnOnRow: function (idx) {
            var grid = this, options = this.Options;
            if (options.CanShowRowNumber) {
                var rownum_column = $("<td></td>");
                var rownum_div = $("<div></div>").addClass("frame-grid-sortable").addClass("frame-grid-sortable-num").text(idx + 1);
                rownum_column.append(rownum_div);
                $("tr.frame-grid-row[r_idx=" + idx + "]", grid.FreezeTable).append(rownum_column);
            }
        },
        _CreateGroovyRow: function (idx, data) {
            var grid = this, options = this.Options;
            var row = $("<tr></tr>").attr("r_idx", idx).addClass("frame-grid-row").addClass("frame-grid-row-ltr");
            $("tbody", grid.GroovyTable).append(row);

            if (options.Columns.length > 0) {
                $(options.Columns).each(function () {
                    grid._PackCellFetchRow(idx, grid.GroovyTable, this, data);
                });
            }
        },
        _PackCellFetchRow: function (idx, table, column, record) {
            ///<summary>
            /// 包装渲染每一行的单元格
            ///</summary>
            ///<param name="idx" type="number">指定行索引。</param>
            ///<param name="table" type="selector">表格jQuery对象。</param>
            ///<param name="column" type="object">列对象。</param>
            ///<param name="record" type="object">行记录数据对象</param>
            var grid = this, options = this.Options;
            var row = $("tr.frame-grid-row[r_idx=" + idx + "]", table);

            var value = record[column.Field];
            var cell = $("<td></td>");
            var column_div = $("<div></div>").addClass("frame-grid-sortable").text(value);
            row.append(cell);
            cell.append(column_div);

            if (column.Formatter) {
                var result = column.Formatter.call(column.Formatter, idx, record, value);
                value = result || value;
                column_div.text(value);
            }
            if (column.Styler) {
                var style = column.Styler.call(column.Styler, idx, record, value);
                if (style) {
                    var reg = /<(\w+)[^>]*>.*<\/\1>/; //匹配HTML标签字符串
                    if (reg.test(style)) {
                        column_div.empty();
                        column_div = column_div.append($(style));
                    }
                    else {
                        column_div.attr("style", style);
                    }
                }
            }

            column_div.width(column.Width);
            if (column.Align)
                column_div.css("text-align", column.Align);

        },
        Resize: function () {
            var grid = this, options = this.Options;

            // 计算常规表格部分可得宽度
            var freezeWidth = grid.FreezeViewContainer.outerWidth(true);    // 这里获取冻结表格部分的宽度
            var groovyWidth = grid.ViewContainer.width() - freezeWidth - 1; // 用表格总宽度-冻结表格部分的宽度,得到常规表格的宽度

            grid.GroovyHeadBox.width(groovyWidth);         // 设置常规表格的表头容器宽度
            grid.GroovyContentBox.width(groovyWidth);      // 设置常规表格表体容器宽度
            grid.GroovyContentContainer.width(groovyWidth); // 设置常规表格表体主容器宽度

            // 计算并设置表格表体实际宽度
            var headHeight = grid.FreezeHeadContainer.outerHeight(true);
            if (!options.CanShowCheckBox && !options.CanShowRowNumber && options.FrozenColumns.length == 0) {
                headHeight = grid.GroovyHeadContainer.outerHeight(true);
            }
            var bodyHeight = grid.ViewContainer.height() - headHeight;

            // 设置冻结表格部分的表体高度
            grid.FreezeContentContainer.height(bodyHeight);
            grid.FreezeContentBox.height(bodyHeight);
            // 设置常规表格部分的表体高度
            grid.GroovyContentContainer.height(bodyHeight);
            grid.GroovyContentBox.height(bodyHeight);

            grid._SetScroll(groovyWidth, bodyHeight);
            grid._ApplyScroll();
        },
        _SetScroll: function (width, height) {
            var grid = this, options = this.Options;

            var twidth = grid.GroovyTable.width();
            var theight = grid.GroovyTable.height();
            if (twidth > width) {
                // 出现水平滚动条
                grid.FreezeContentBox.height(height - 17);
                grid.GroovyContentBox.height(height - 17);
            }

            if (theight > height) {
                // 出现垂直滚动条
                if (twidth > width) {
                    grid.GroovyHeadBox.width(width - 17);
                }
                grid.GroovyContentBox.width(twidth);
            }
        },
        _ApplyScroll: function () {
            var grid = this, options = this.Options;
            grid.GroovyContentContainer.scroll(function () {
                grid.FreezeContentBox.scrollTop(grid.GroovyContentContainer.scrollTop());
                grid.GroovyHeadBox.scrollLeft(grid.GroovyContentContainer.scrollLeft());
            });
        },
        LoadData: function (data) {
            ///<summary>
            /// 加载本地数据,旧记录将会被删除
            ///</summary>
            ///<param name="data" type="object">本地数据对象,对象格式:{Total,Rows}。</param>
            var grid = this, options = this.Options;
            grid.Loading = true;
            if (grid.HasBind("Extractor")) {
                data = grid.Trigger("Extractor", [data]);
            }

            grid.Clear();

            if (grid.HasBind("BeforeShowData")) {
                data = grid.Trigger("BeforeShowData", [data]);
                if (!data)
                    return;
            }
            grid.Set({ DataSource: data });

            if (grid.HasBind("AfterShowData")) {
                grid.Trigger("AfterShowData", []);
            }
            grid.Resize();
        },
        Clear: function () {
            ///<summary>
            /// 清空数据表格的数据,若有启用分页,则同时设置分页初始状态
            ///</summary>
            var grid = this, options = this.Options;

            grid.DataSource = [];
            grid.Selections = [];
            $("tbody", grid.FreezeTable).empty();
            $("tbody", grid.GroovyTable).empty();
            if (options.CanShowPager)
                grid.Pagination.Refresh(0, 0);
        },
        Load: function (param) {
            ///<summary>
            /// 从服务器远程请求数据加载
            ///</summary>
            ///<param name="param" type="object">请求参数,两种参数格式:1.Url,2.Url、{"":""}</param>
            var grid = this, options = this.Options;
            if (arguments.length > 1) {
                options.Url = arguments[0];
                param = arguments[1];
            }
            if (arguments.length == 1 && typeof arguments[0] == "string") {
                options.Url = arguments[0];
                param = {};
            }
            $.extend(param, options.Params);
            if (grid.Loading || !options.Url)
                return;

            $.ajax({
                dataType: options.DataType, type: options.Method, url: options.Url, data: param,
                beforeSend: function () {
                    if (grid.HasBind("Loading")) {
                        grid.Trigger("Loading");
                    }
                    else {
                        if (options.CanShowLoading)
                            grid._ShowLoading();
                    }
                },
                success: function (data) {
                    if (grid.HasBind("Success")) {
                        data = grid.Trigger("Success", [data]);
                    }
                    if (data != null) {
                        grid.LoadData(data);
                    }
                },
                error: function (xhr, info, thrown) {
                    if (grid.HasBind("Error")) {
                        grid.Trigger("Error", [xhr, info, thrown]);
                    }
                    if (info == "error") {
                        $.Messager.Alert("错误提示", thrown, "error");
                    }
                },
                complete: function (xhr, ts) {
                    grid.Loading = false;
                    grid.Trigger("Complete", [xhr, ts]);
                    if (ts == "success") {
                        if (grid.HasBind("Loaded")) {
                            grid.Trigger("Loaded", [xhr, ts]);
                        }
                    }
                    if (options.CanShowLoading)
                        grid._CloseLoading();
                }
            });

        },
        SelectRow: function (index) {
            /// <summary>
            /// 选中一行数据
            ///</summary>
            ///<param name="index" type="number">行索引,索引从0起始</param>
            var grid = this, options = this.Options;

            var idx = grid._InSelected(index);

            if (idx == -1) {
                // 还未被选中
                var row_freeze = $("tr.frame-grid-row[r_idx=" + index + "]", grid.FreezeTable);
                var row_groovy = $("tr.frame-grid-row[r_idx=" + index + "]", grid.GroovyTable);
                grid._RowOnClick(index, row_freeze, row_groovy);
            }
        },
        UnSelectRow: function (index) {
            var grid = this, options = this.Options;

            var idx = grid._InSelected(index);

            if (idx > -1) {
                var row_freeze = $("tr.frame-grid-row[r_idx=" + index + "]", grid.FreezeTable);
                var row_groovy = $("tr.frame-grid-row[r_idx=" + index + "]", grid.GroovyTable);
                grid._RowOnClick(index, row_freeze, row_groovy);
            }
        },
        GetSelections: function () {
            /// <summary>
            /// 获取选中的数据记录
            ///</summary>
            var grid = this, options = this.Options;

            if (grid.Selections.length > 0) {
                return grid.Selections;
            }
            else
                return false;
        },
        _FindRecordData: function (index) {
            /// <summary>
            /// 查找并获取指定索引的行数据
            ///</summary>
            ///<param name="index" type="number">行索引</param>
            var grid = this, options = this.Options;
            if (grid.DataSource && grid.DataSource.length > 0) {
                return grid.DataSource[index];
            }
            else
                return -1;
        },
        _AppendRecord: function (index) {
            var grid = this, options = this.Options;
            var idx = grid._InSelected(index);

            if (idx == -1) {
                var row = grid._FindRecordData(index);
                grid.Selections.push(row);
            }
        },
        _RemoveRecord: function (index) {
            var grid = this, options = this.Options;
            var idx = grid._InSelected(index);
            if (idx > -1) {
                grid.Selections.splice(idx, 1);
            }
        },
        _ToggleRecord: function (index) {
            var grid = this, options = this.Options;
            var idx = grid._InSelected(index);
            if (idx == -1) {
                var row = grid._FindRecordData(index);
                grid.Selections.push(row);

                if (grid.HasBind("SelectRow"))
                    grid.Trigger("SelectRow", [index]);
            }
            if (idx > -1) {
                grid.Selections.splice(idx, 1);

                if (grid.HasBind("UnSelectRow"))
                    grid.Trigger("UnSelectRow", [index]);
            }
        },
        _InSelected: function (index) {
            /// <summary>
            /// 返回一个值，该值标识指定索引的数据行是否被选中
            ///</summary>
            ///<param name="index" type="number">行索引</param>
            var grid = this, options = this.Options;
            return $.inArray(grid._FindRecordData(index), grid.Selections);
        },
        SelectAll: function () {
            var grid = this, options = this.Options;
            if (options.CanMultiple) {
                if (options.FrozenColumns.length > 0) {
                    grid._SelectAllByFreezeTable();
                } else {
                    grid._SelectAllByGroovyTable();
                }
            }
        },
        UnSelectAll: function () {
            var grid = this, options = this.Options;
            if (options.CanMultiple) {
                if (options.FrozenColumns.length > 0) {
                    grid._UnSelectAllByFreezeTable();
                } else {
                    grid._UnSelectAllByGroovyTable();
                }
            }
        },
        _SelectAllByFreezeTable: function () {
            var grid = this, options = this.Options;
            $("tbody", grid.FreezeTable).children().each(function () {
                var row_freeze = $(this);
                var row_idx = row_freeze.attr("r_idx");
                var checkstate = options.CanShowCheckBox == true ?
                        $("#" + grid.Id + "_row_chk_" + row_idx).Frame("Options", "Checked") :
                        row_freeze.hasClass("frame-grid-state-highlight");
                if (!checkstate) {
                    var row_groovy = $("tr.frame-grid-row[r_idx=" + row_idx + "]", grid.GroovyTable);
                    grid._RowOnClick(row_idx, row_freeze, row_groovy);
                }
            });
        },
        _SelectAllByGroovyTable: function () {
            var grid = this, options = this.Options;
            $("tbody", grid.GroovyTable).children().each(function () {
                var row_groovy = $(this);
                var row_idx = row_groovy.attr("r_idx");
                if (!row_groovy.hasClass("frame-grid-state-highlight")) {
                    var row_freeze = $("tr.frame-grid-row[r_idx=" + row_idx + "]", grid.FreezeTable);
                    grid._RowOnClick(row_idx, row_freeze, row_groovy);
                }
            });
        },
        _UnSelectAllByFreezeTable: function () {
            var grid = this, options = this.Options;
            $("tbody", grid.FreezeTable).children().each(function () {
                var row_freeze = $(this);
                var row_idx = row_freeze.attr("r_idx");
                var checkstate = options.CanShowCheckBox == true ?
                        $("#" + grid.Id + "_row_chk_" + row_idx).Frame("Options", "Checked") :
                        row_freeze.hasClass("frame-grid-state-highlight");
                if (checkstate) {
                    var row_groovy = $("tr.frame-grid-row[r_idx=" + row_idx + "]", grid.GroovyTable);
                    grid._RowOnClick(row_idx, row_freeze, row_groovy);
                }
            });

        },
        _UnSelectAllByGroovyTable: function () {
            var grid = this, options = this.Options;
            $("tbody", grid.GroovyTable).children().each(function () {
                var row_groovy = $(this);
                var row_idx = row_groovy.attr("r_idx");
                if (row_groovy.hasClass("frame-grid-state-highlight")) {
                    var row_freeze = $("tr.frame-grid-row[r_idx=" + row_idx + "]", grid.FreezeTable);
                    grid._RowOnClick(row_idx, row_freeze, row_groovy);
                }
            });
        },
        GetSelected: function () {
            ///<summary>
            /// 获取当前选中记录的第一行数据行
            ///</summary>
            var grid = this, options = this.Options;
            if (grid.Selections.length > 0) {
                return grid.Selections[0];
            }

            return false;
        }
    });
})(jQuery);