/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.13
* 说明：DatePicker 日期控件。
*/
(function ($) {
    $.fn.DatePicker = function (options) {
        return $.Frame.Do.call(this, "DatePicker", arguments);
    };

    $.Methods.DatePicker = $.Methods.DatePicker || {};

    $.Defaults.DatePicker = {
        Width: 165,
        ShowType: "ymd", // 显示控件类型：ymd,ym
        DateFormat: "yyyy-MM-dd hh:mm", //在日期文本框中显示的日期格式 yyyy-MM-dd hh:mm/yyyy-MM-dd等
        IsShowDate: true, //是否默认显示日期
        CanShowTime: true, // 是否显示出时间
        Disabled: false,
        ReadOnly: false,
        OnExtractor: null,  // 对日期值作进一步格式处理,参数为date(string)
        OnChange: null,
        OnResolve: null     // 对设置传入的日期值进行格式反转，以使其符合内部运行格式
    };

    $.Frame.Controls.DatePicker = function (element, options) {
        $.Frame.Controls.DatePicker.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.DatePicker.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "DatePicker";
        },
        _IdPrev: function () {
            return "DatePickerFor";
        },
        _ExtendMethods: function () {
            return $.Methods.DatePicker;
        },
        _PreRender: function () {
            var datepicker = this;

            if (!$(datepicker.Element).hasClass("frame-datepicker")) {
                $(datepicker.Element).addClass("frame-datepicker");
            }

            datepicker.Calendar = null;
            datepicker.CalendarId = "dp_" + datepicker.Id;

            var date = new Date();

            // 当前日期
            datepicker.Now = {
                Year: date.getFullYear().toString(),
                Month: (date.getMonth() + 1) < 10 ? ("0" + (date.getMonth() + 1)) : (date.getMonth() + 1).toString(),
                Day: date.getDate() < 10 ? "0" + date.getDate() : date.getDate().toString(),
                Hour: date.getHours() < 10 ? "0" + date.getHours() : date.getHours().toString(),
                Minute: date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes().toString(),
                PrevYear: function (value) {
                    this.Year = (parseInt(this.Year) - (value ? value : 1)).toString();
                    return this;
                },
                PrevMonth: function () {
                    var month = parseInt(this.Month) - 1;
                    if (month == 0) {
                        month = 12;
                        this.PrevYear();
                    }
                    this.Month = month < 10 ? "0" + month : month.toString();
                    return this;
                },
                NextMonth: function () {
                    var month = parseInt(this.Month) + 1;
                    if (month > 12) {
                        month = 1;
                        this.NextYear();
                    }
                    this.Month = month < 10 ? "0" + month : month.toString();
                    return this;
                },
                NextYear: function (value) {
                    this.Year = (parseInt(this.Year) + (value ? value : 1)).toString();
                    return this;
                },
                ToDateTime: function () {
                    var date = null;
                    if (datepicker.Options.CanShowTime)
                        date = new Date(this.Year + "/" + this.Month + "/" + this.Day + " " + this.Hour + ":" + this.Minute);
                    else
                        date = new Date(this.Year + "/" + this.Month + "/" + this.Day);

                    return date;
                }
            };

            datepicker._PageYear = parseInt(datepicker.Now.Year.substring(0, 2) + "09") + 10;
            datepicker.ActiveDate = {
                Year: datepicker.Now.Year,
                Month: datepicker.Now.Month,
                Day: datepicker.Now.Day,
                Hour: datepicker.Now.Hour,
                Minute: datepicker.Now.Minute,
                PrevYear: function (value) {
                    this.Year = (parseInt(this.Year) - (value ? value : 1)).toString();
                    return this;
                },
                PrevMonth: function () {
                    var month = parseInt(this.Month) - 1;
                    if (month == 0) {
                        month = 12;
                        this.PrevYear();
                    }
                    this.Month = month < 10 ? "0" + month : month.toString();
                    return this;
                },
                NextMonth: function () {
                    var month = parseInt(this.Month) + 1;
                    if (month > 12) {
                        month = 1;
                        this.NextYear();
                    }
                    this.Month = month < 10 ? "0" + month : month.toString();
                    return this;
                },
                NextYear: function (value) {
                    this.Year = (parseInt(this.Year) + (value ? value : 1)).toString();
                    return this;
                },
                ToDateTime: function () {
                    var date = null;
                    if (datepicker.Options.CanShowTime)
                        date = new Date(this.Year + "/" + this.Month + "/" + this.Day + " " + this.Hour + ":" + this.Minute);
                    else
                        date = new Date(this.Year + "/" + this.Month + "/" + this.Day);

                    return date;
                }
            };
        },
        _Render: function () {
            var datepicker = this, options = this.Options;

            datepicker.Container = $("<div></div>").addClass("frame-datepicker-container");
            $(datepicker.Element).wrap(datepicker.Container);
            datepicker.AddonButton = $("<span></span>").addClass("frame-datepicker-addon")
                                                       .append('<i class="icon-calendar bigger-110"></i>')
                                                       .on("click", function () {
                                                           if (datepicker.Disabled)
                                                               return false;
                                                           $(datepicker.Element).focus();
                                                           datepicker._ResetActiveDate();
                                                           datepicker.Set(options);
                                                           datepicker._OnCalendarResize();
                                                           $("#" + datepicker.CalendarId).slideToggle("fast");
                                                           return false;
                                                       });
            $(datepicker.Element).after(datepicker.AddonButton);
            $(datepicker.Element).on("keyup.DatePicker change.DatePicker", function (e) {
                if (datepicker.HasBind("Change"))
                    datepicker.Trigger("Change", [this.value]);
                var v = datepicker._Resolve(this.value);
                if (v != null) {
                    datepicker._ResetActiveDate();
                    datepicker.Set(options);
                }
                return false;
            });

            datepicker._CreateCalendar();
            datepicker.Set(options);
        },
        _ResetActiveDate: function () {
            var datepicker = this, options = this.Options;
            datepicker.ActiveDate = $.extend(datepicker.ActiveDate, datepicker.Now);
            datepicker._PageYear = parseInt(datepicker.Now.Year.substring(0, 2) + "09") + 10;
        },
        _CreateCalendar: function () {
            var datepicker = this, options = this.Options;

            datepicker.Calendar = $("<div></div>").addClass("frame-datepicker-panel")
                                                  .attr("id", datepicker.CalendarId).appendTo("body");

            datepicker._CreateCalendarForDays();
            datepicker._CreateCalendarForMonths();
            datepicker._CreateCalendarForYears();
            datepicker._CreateCalendarForHours();
            datepicker._CreateCalendarForMinutes();

            datepicker._CreateCalendarFooter();
        },
        _CreateCalendarFooter: function () {
            var datepicker = this, options = this.Options;

            datepicker.Calendar.append($("<div></div>").addClass("frame-datepicker-panel-foot"));
            datepicker.Calendar.TimeContainer = $("<div></div>").addClass("frame-datepicker-panel-time").hide();

            $(".frame-datepicker-panel-foot", datepicker.Calendar).append(datepicker.Calendar.TimeContainer);
            $("<label></label>").addClass("frame-label").addClass("frame-label-transparent").addClass("input-mini")
                                .addClass("for-hour").text(datepicker.Now.Hour)
                                .appendTo(datepicker.Calendar.TimeContainer)
                                .on("click", function () {
                                    if (!$(".frame-datepicker-hours", datepicker.Calendar).is(":visible")) {
                                        $(".frame-datepicker-days", datepicker.Calendar).hide();
                                        $(".frame-datepicker-minutes", datepicker.Calendar).hide();
                                        $(".frame-datepicker-hours", datepicker.Calendar).show();
                                        datepicker._BuildCalendarContentForHours();
                                    } else {
                                        $(".frame-datepicker-days", datepicker.Calendar).show();
                                        $(".frame-datepicker-hours", datepicker.Calendar).hide();
                                    }
                                });
            datepicker.Calendar.TimeContainer.append("<span>:</span>");
            $("<label></label>").addClass("frame-label").addClass("frame-label-transparent").addClass("input-mini")
                                .addClass("for-minute").text(datepicker.Now.Minute)
                                .appendTo(datepicker.Calendar.TimeContainer)
                                .on("click", function () {
                                    if (!$(".frame-datepicker-minutes", datepicker.Calendar).is(":visible")) {
                                        $(".frame-datepicker-days", datepicker.Calendar).hide();
                                        $(".frame-datepicker-hours", datepicker.Calendar).hide();
                                        $(".frame-datepicker-minutes", datepicker.Calendar).show();
                                        datepicker._BuildCalendarContentForMinutes();
                                    } else {
                                        $(".frame-datepicker-days", datepicker.Calendar).show();
                                        $(".frame-datepicker-minutes", datepicker.Calendar).hide();
                                    }
                                });
            $(".frame-datepicker-panel-foot", datepicker.Calendar).append($('<span class="today">关闭</span>')
                                .on("click", function () {
                                    $("#" + datepicker.CalendarId).slideUp("fast");
                                    return false;
                                }));
        },
        _GetWeekOnFirstDay: function () {
            ///<summary>
            /// 获取指定日期(默认当前日期)当月第一天对应的星期
            /// 0:周日,1:周一,2:周二,3:周三,4:周四,5:周五,6:周六
            ///</summary>
            var datepicker = this, options = this.Options;
            var dateString = datepicker.ActiveDate.ToDateTime().Format("yyyy/MM/1");
            return new Date(dateString).getDay();
        },
        _GetDaysOfDate: function () {
            ///<summary>
            /// 获取当前日期月份的天数
            ///</summary>
            var datepicker = this, options = this.Options;
            //var dateString = datepicker.ActiveDate.ToDateTime().AddMonth(1).Format("yyyy/MM/0");
            //return new Date(dateString).getDate();
            var year = parseInt(datepicker.ActiveDate.Year);
            var month = parseInt(datepicker.ActiveDate.Month);
            return new Date(year, month, 0).getDate();
        },
        _GetLastDaysOfDate: function () {
            ///<summary>
            /// 获取当前日期的上一个月天数
            ///</summary>
            var datepicker = this, options = this.Options;
            //var dateString = datepicker.ActiveDate.ToDateTime().Format("yyyy/MM/0");
            //return new Date(dateString).getDate();
            var year = parseInt(datepicker.ActiveDate.Year);
            var month = parseInt(datepicker.ActiveDate.ToDateTime().AddMonth(-1).getMonth() + 1);
            return new Date(year, month, 0).getDate();
        },
        _SetHeadText: function (table) {
            var datepicker = this, options = this.Options;

            var headtext = datepicker.ActiveDate.Year + "年" + datepicker.ActiveDate.Month + "月";
            $("thead tr", table).first().children(".switch").text(headtext);
        },
        _CreateCalendarForDays: function () {
            var datepicker = this, options = this.Options;
            var container = $("<div></div>").addClass("frame-datepicker-days").appendTo(datepicker.Calendar);
            var table = $("<table></table>").addClass("table-condensed").append("<thead></thead><tbody></tbody>")
                        .appendTo(container);
            $("thead", table).append("<tr></tr>");
            $("thead > tr", table).append($("<th></th>").addClass("prev").css("visibility", "visible").append($("<i></i>").addClass("icon-arrow-left"))
                                                        .on("click", function () {
                                                            datepicker.ActiveDate.PrevMonth();
                                                            datepicker._BuildCalendarContentForDays();
                                                        }));
            $("thead > tr", table).append($('<th colspan="5"></th>').addClass("switch")
                                                                    .on("click", function () {
                                                                        datepicker._SetCanShowTime(false);
                                                                        $(".frame-datepicker-days", datepicker.Calendar).hide();
                                                                        $(".frame-datepicker-months", datepicker.Calendar).show();
                                                                        datepicker._BuildCalendarContentForMonths();
                                                                    }));
            $("thead > tr", table).append($("<th></th>").addClass("next").css("visibility", "visible").append($("<i></i>").addClass("icon-arrow-right"))
                                                        .on("click", function () {
                                                            datepicker.ActiveDate.NextMonth();
                                                            datepicker._BuildCalendarContentForDays();
                                                        }));

            $("thead", table).append("<tr></tr>");
            $("thead tr:last", table).append($("<th>日</th>").addClass("dow"))
                                                   .append($("<th>一</th>").addClass("dow"))
                                                   .append($("<th>二</th>").addClass("dow"))
                                                   .append($("<th>三</th>").addClass("dow"))
                                                   .append($("<th>四</th>").addClass("dow"))
                                                   .append($("<th>五</th>").addClass("dow"))
                                                   .append($("<th>六</th>").addClass("dow"));

            $("tbody", table).append($("<tr></tr>").append('<td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td>'));
            $("tbody", table).append($("<tr></tr>").append('<td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td>'));
            $("tbody", table).append($("<tr></tr>").append('<td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td>'));
            $("tbody", table).append($("<tr></tr>").append('<td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td>'));
            $("tbody", table).append($("<tr></tr>").append('<td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td>'));
            $("tbody", table).append($("<tr></tr>").append('<td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td><td class="day"></td>'));

        },
        _BuildCalendarContentForDays: function () {
            var datepicker = this, options = this.Options;

            datepicker.Set({ CanShowTime: options.CanShowTime });
            // 当前日期第一天星期
            var week = datepicker._GetWeekOnFirstDay();
            // 当前日期天数
            var days = datepicker._GetDaysOfDate();
            // 上一个月天数
            var lastDays = datepicker._GetLastDaysOfDate();

            var table = $(".frame-datepicker-days > .table-condensed", datepicker.Calendar);
            datepicker._SetHeadText(table);

            $("tbody td.active", table).removeClass("active");
            $("tbody td", table).off("click");
            $("tbody", table).children().each(function (idx) {
                var day;
                if (idx == 0 && week > 0) {
                    for (var index = 1; index <= week; index++) {
                        day = lastDays - (week - index);
                        $(this).children().eq(index - 1).text(day).addClass("old")
                            .on("click", function () {
                                datepicker.ActiveDate.PrevMonth();
                                datepicker.Now.Month = datepicker.ActiveDate.Month;
                                datepicker.ActiveDate.Day = parseInt($(this).text()) < 10 ? "0" + $(this).text() : $(this).text();
                                datepicker.Now.Day = datepicker.ActiveDate.Day;
                                datepicker._SetDate();
                                $("#" + datepicker.CalendarId).slideUp("fast");
                                datepicker._BuildCalendarContentForDays();
                                return false;
                            });
                    }
                }
                for (var i = (idx == 0 ? week : 0); i < 7; i++) {
                    day = (idx * 7) + (i - week) + 1;
                    var toggle = (day <= days);
                    $(this).children().eq(i).text(toggle ? day : day - days).removeClass("old").toggleClass("new", !toggle)
                            .on("click", function () {
                                if ($(this).hasClass("new")) {
                                    datepicker.ActiveDate.NextMonth();
                                    datepicker.Now.Month = datepicker.ActiveDate.Month;
                                }

                                datepicker.Now.Year = datepicker.ActiveDate.Year;
                                datepicker.Now.Month = datepicker.ActiveDate.Month;
                                datepicker.Now.Hour = datepicker.ActiveDate.Hour;
                                datepicker.Now.Minute = datepicker.ActiveDate.Minute;
                                datepicker.Now.Day = parseInt($(this).text()) < 10 ? "0" + $(this).text() : $(this).text();
                                datepicker.ActiveDate.Day = datepicker.Now.Day;
                                datepicker._SetDate();
                                $("#" + datepicker.CalendarId).slideUp("fast");
                                datepicker._BuildCalendarContentForDays();
                                return false;
                            });
                    if (datepicker.Now.Year == datepicker.ActiveDate.Year &&
                    datepicker.Now.Month == datepicker.ActiveDate.Month &&
                    day == parseInt(datepicker.Now.Day)) {
                        $(this).children().eq(i).addClass("active");
                    }
                }
            });
        },
        _CreateCalendarForMonths: function () {
            var datepicker = this, options = this.Options;

            var container = $("<div></div>").addClass("frame-datepicker-months").appendTo(datepicker.Calendar);
            var table = $("<table></table>").addClass("table-condensed").append("<thead></thead><tbody></tbody>")
                        .appendTo(container);

            $("thead", table).append("<tr></tr>");
            $("thead > tr", table).append($("<th></th>").addClass("prev").css("visibility", "hidden").append($("<i></i>").addClass("icon-arrow-left")));
            $("thead > tr", table).append($('<th colspan="5"></th>').addClass("switch")
                                                                    .on("click", function () {
                                                                        $(".frame-datepicker-months", datepicker.Calendar).hide();
                                                                        $(".frame-datepicker-years", datepicker.Calendar).show();
                                                                        datepicker._BuildCalendarContentForYears();
                                                                    }));
            $("thead > tr", table).append($("<th></th>").addClass("next").css("visibility", "hidden").append($("<i></i>").addClass("icon-arrow-right")));

            $("tbody", table).append("<tr></tr>");
            $("tbody > tr", table).append('<td colspan="7"><span class="month">1月</span><span class="month">2月</span>'
                + '<span class="month">3月</span><span class="month">4月</span><span class="month">5月</span>'
                + '<span class="month">6月</span><span class="month">7月</span><span class="month">8月</span>'
                + '<span class="month">9月</span><span class="month">10月</span><span class="month">11月</span>'
                + '<span class="month">12月</span></td>');

            datepicker._BuildCalendarContentForMonths();
        },
        _BuildCalendarContentForMonths: function () {
            var datepicker = this, options = this.Options;

            var table = $(".frame-datepicker-months > .table-condensed", datepicker.Calendar);

            var headtext = datepicker.ActiveDate.Year + "年";
            $("thead tr", table).first().children(".switch").text(headtext);

            $("tbody > tr > td span.active", table).removeClass("active");

            var mstring = parseInt(datepicker.Now.Month) + "月";
            $("tbody > tr > td", table).children().each(function () {
                if (datepicker.Now.Year == datepicker.ActiveDate.Year && $(this).text() == mstring)
                    $(this).addClass("active");
            }).on("click", function () {
                var v = parseInt($(this).text().match(/[1-9][0-9]*/g)[0]);
                if (options.ShowType == "ymd") {
                    datepicker.ActiveDate.Month = v < 10 ? "0" + v : v;
                    datepicker._BuildCalendarContentForDays();
                    $(".frame-datepicker-months", datepicker.Calendar).hide();
                    $(".frame-datepicker-days", datepicker.Calendar).show();
                }
                else {
                    datepicker.Now.Month = v < 10 ? "0" + v : v;
                    datepicker._SetDate();
                    $("#" + datepicker.CalendarId).slideUp("fast");
                }
                return false;
            });
        },
        _CreateCalendarForYears: function () {
            var datepicker = this, options = this.Options;

            var container = $("<div></div>").addClass("frame-datepicker-years").appendTo(datepicker.Calendar);
            var table = $("<table></table>").addClass("table-condensed").append("<thead></thead><tbody></tbody>")
                        .appendTo(container);

            $("thead", table).append("<tr></tr>");
            $("thead > tr", table).append($("<th></th>").addClass("prev").css("visibility", "visible").append($("<i></i>").addClass("icon-arrow-left"))
                                                        .on("click", function () {
                                                            datepicker._PageYear -= 10;
                                                            datepicker._BuildCalendarContentForYears();
                                                        }));
            $("thead > tr", table).append($('<th colspan="5"></th>').addClass("switch"));
            $("thead > tr", table).append($("<th></th>").addClass("next").css("visibility", "visible").append($("<i></i>").addClass("icon-arrow-right"))
                                                        .on("click", function () {
                                                            datepicker._PageYear += 10;
                                                            datepicker._BuildCalendarContentForYears();
                                                        }));

            $("tbody", table).append("<tr></tr>");
            $("tbody > tr", table).append('<td colspan="7"><span class="year"></span><span class="year"></span>'
                + '<span class="year"></span><span class="year"></span><span class="year"></span>'
                + '<span class="year"></span><span class="year"></span><span class="year"></span>'
                + '<span class="year"></span><span class="year"></span><span class="year"></span>'
                + '<span class="year"></span></td>');

            datepicker._BuildCalendarContentForYears();
        },
        _BuildCalendarContentForYears: function () {
            var datepicker = this, options = this.Options;

            var year = datepicker._PageYear;
            var start = year - 10;

            var table = $(".frame-datepicker-years > .table-condensed", datepicker.Calendar);

            var headtext = start + "-" + year;
            $("thead tr", table).first().children(".switch").text(headtext);
            $("tbody > tr > td span.active", table).removeClass("active");

            $("tbody > tr > td", table).children().each(function (i) {
                $(this).text(start + i);
                if ((start + i) == datepicker.Now.Year)
                    $(this).addClass("active");
            }).on("click", function () {
                datepicker.ActiveDate.Year = $(this).text();
                $(".frame-datepicker-months", datepicker.Calendar).show();
                $(".frame-datepicker-years", datepicker.Calendar).hide();
                datepicker._BuildCalendarContentForMonths();
            });
        },
        _CreateCalendarForHours: function () {
            var datepicker = this, options = this.Options;
            var container = $("<div></div>").addClass("frame-datepicker-hours").appendTo(datepicker.Calendar);
            var table = $("<table></table>").addClass("table-condensed").append("<thead></thead><tbody></tbody>")
                        .appendTo(container);
            $("thead", table).append("<tr></tr>");
            $("thead > tr", table).append($("<th></th>").addClass("prev").css("visibility", "hidden").append($("<i></i>").addClass("icon-arrow-left")));
            $("thead > tr", table).append($('<th colspan="5"></th>').addClass("switch"));
            $("thead > tr", table).append($("<th></th>").addClass("next").css("visibility", "hidden").append($("<i></i>").addClass("icon-arrow-right")));

            $("tbody", table).append("<tr></tr>");
            $("tbody > tr", table).append('<td colspan="7"><span class="hour">00</span><span class="hour">01</span><span class="hour">02</span>'
                + '<span class="hour">03</span><span class="hour">04</span><span class="hour">05</span>'
                + '<span class="hour">06</span><span class="hour">07</span><span class="hour">08</span>'
                + '<span class="hour">09</span><span class="hour">10</span><span class="hour">11</span>'
                + '<span class="hour">12</span><span class="hour">13</span><span class="hour">14</span>'
                + '<span class="hour">15</span><span class="hour">16</span><span class="hour">17</span>'
                + '<span class="hour">18</span><span class="hour">19</span><span class="hour">20</span>'
                + '<span class="hour">21</span><span class="hour">22</span><span class="hour">23</span></td>');

            datepicker._BuildCalendarContentForHours();
        },
        _BuildCalendarContentForHours: function () {
            var datepicker = this, options = this.Options;
            var table = $(".frame-datepicker-hours > .table-condensed", datepicker.Calendar);
            datepicker._SetHeadText(table);

            $("tbody > tr > td span.active", table).removeClass("active");
            $("tbody > tr > td", table).children().each(function (i) {
                if ($(this).text() == datepicker.Now.Hour)
                    $(this).addClass("active");
            }).on("click", function () {
                datepicker.ActiveDate.Hour = $(this).text();
                $("label.for-hour", datepicker.Calendar.TimeContainer).text(datepicker.ActiveDate.Hour);
                $(".frame-datepicker-days", datepicker.Calendar).show();
                $(".frame-datepicker-hours", datepicker.Calendar).hide();
            });
        },
        _CreateCalendarForMinutes: function () {
            var datepicker = this, options = this.Options;
            var container = $("<div></div>").addClass("frame-datepicker-minutes").appendTo(datepicker.Calendar);
            var table = $("<table></table>").addClass("table-condensed").append("<thead></thead><tbody></tbody>")
                        .appendTo(container);
            $("thead", table).append("<tr></tr>");
            $("thead > tr", table).append($("<th></th>").addClass("prev").css("visibility", "visible").append($("<i></i>").addClass("icon-arrow-left")));
            $("thead > tr", table).append($('<th colspan="5"></th>').addClass("switch"));
            $("thead > tr", table).append($("<th></th>").addClass("next").css("visibility", "visible").append($("<i></i>").addClass("icon-arrow-right")));

            $("tbody", table).append("<tr></tr>");
            $("tbody > tr", table).append('<td colspan="7"><span class="minute">00</span><span class="minute">01</span>'
                + '<span class="minute">02</span><span class="minute">03</span><span class="minute">04</span>'
                + '<span class="minute">05</span><span class="minute">06</span><span class="minute">07</span>'
                + '<span class="minute">08</span><span class="minute">09</span><span class="minute">10</span>'
                + '<span class="minute">11</span><span class="minute">12</span><span class="minute">13</span>'
                + '<span class="minute">14</span><span class="minute">15</span><span class="minute">16</span>'
                + '<span class="minute">17</span><span class="minute">18</span><span class="minute">19</span>'
                + '<span class="minute">20</span><span class="minute">21</span><span class="minute">22</span>'
                + '<span class="minute">23</span><span class="minute">24</span><span class="minute">25</span>'
                + '<span class="minute">26</span><span class="minute">27</span><span class="minute">28</span>'
                + '<span class="minute">29</span><span class="minute">30</span><span class="minute">31</span>'
                + '<span class="minute">32</span><span class="minute">33</span><span class="minute">34</span>'
                + '<span class="minute">35</span><span class="minute">36</span><span class="minute">37</span>'
                + '<span class="minute">38</span><span class="minute">39</span><span class="minute">40</span>'
                + '<span class="minute">41</span><span class="minute">42</span><span class="minute">43</span>'
                + '<span class="minute">44</span><span class="minute">45</span><span class="minute">46</span>'
                + '<span class="minute">47</span><span class="minute">48</span><span class="minute">49</span>'
                + '<span class="minute">50</span><span class="minute">51</span><span class="minute">52</span>'
                + '<span class="minute">53</span><span class="minute">54</span><span class="minute">55</span>'
                + '<span class="minute">56</span><span class="minute">57</span><span class="minute">58</span>'
                + '<span class="minute">59</span></td>');

            datepicker._BuildCalendarContentForMinutes();
        },
        _BuildCalendarContentForMinutes: function () {
            var datepicker = this, options = this.Options;
            var table = $(".frame-datepicker-minutes > .table-condensed", datepicker.Calendar);
            datepicker._SetHeadText(table);

            $("tbody > tr > td span.active", table).removeClass("active");
            $("tbody > tr > td", table).children().each(function (i) {
                if ($(this).text() == datepicker.Now.Minute)
                    $(this).addClass("active");
            }).on("click", function () {
                datepicker.ActiveDate.Minute = $(this).text();
                $("label.for-minute", datepicker.Calendar.TimeContainer).text(datepicker.ActiveDate.Minute);
                $(".frame-datepicker-days", datepicker.Calendar).show();
                $(".frame-datepicker-minutes", datepicker.Calendar).hide();
            });
        },
        _OnCalendarResize: function () {
            ///<summary>
            /// 重新定位日历部分显示位置
            ///</summary>
            var datepicker = this, options = this.Options;

            var contentHeight = $(document).height();

            var left = $(datepicker.Element).parent().offset().left;
            var top = $(datepicker.Element).parent().offset().top + $(datepicker.Element).parent().outerHeight(true) + 5;

            if (Number(top + datepicker.Calendar.height()) > contentHeight
            && contentHeight > Number(datepicker.Calendar.height() + 1)) {
                //若下拉框大小超过当前document下边框,且当前document上留白大于下拉内容高度,下拉内容向上展现
                top = $(datepicker.Element).parent().offset().top - 1 - datepicker.Calendar.outerHeight(true);
            }

            datepicker.Calendar.css("left", left).css("top", top);
        },
        _SetShowType: function (type) {
            var datepicker = this, options = this.Options;
            switch (type) {
                case "ymd":
                    $(".frame-datepicker-days", datepicker.Calendar).show();
                    $(".frame-datepicker-months", datepicker.Calendar).hide();
                    $(".frame-datepicker-years", datepicker.Calendar).hide();
                    $(".frame-datepicker-hours", datepicker.Calendar).hide();
                    $(".frame-datepicker-minutes", datepicker.Calendar).hide();
                    datepicker._BuildCalendarContentForDays();
                    break;
                case "ym":
                    $(".frame-datepicker-months", datepicker.Calendar).show();
                    $(".frame-datepicker-days", datepicker.Calendar).hide();
                    $(".frame-datepicker-years", datepicker.Calendar).hide();
                    $(".frame-datepicker-hours", datepicker.Calendar).hide();
                    $(".frame-datepicker-minutes", datepicker.Calendar).hide();
                    datepicker._BuildCalendarContentForMonths();
                    break;
            }
        },
        _SetCanShowTime: function (can) {
            var datepicker = this, options = this.Options;
            if (options.ShowType == "ymd") {
                if (can) {
                    $(".frame-datepicker-panel-foot", datepicker.Calendar).addClass("on-time");
                    datepicker.Calendar.TimeContainer.show();
                } else {
                    $(".frame-datepicker-panel-foot", datepicker.Calendar).removeClass("on-time");
                    datepicker.Calendar.TimeContainer.hide();
                }
            }
        },
        _IsDateTime: function (date) {
            ///<summary>
            /// 返回一个值，该值标识日期格式是否符合短日期格式，即yyyy-MM-dd
            ///</summary>
            ///<param name="date" type="string">日期值</param>
            var datepicker = this, options = this.Options;
            var r = date.match(/^(\d{4})(-|\/)(\d{2})\2(\d{2})$/);
            if (r == null) return false;
            var d = new Date(r[1], r[3] - 1, r[4]);
            if (d == "NaN") return false;
            return (d.getFullYear() == r[1] && (d.getMonth() + 1) == r[3] && d.getDate() == r[4]);
        },
        _IsLongDateTime: function (date) {
            ///<summary>
            /// 返回一个值，该值标识日期格式是否符合长日期格式，即yyyy-MM-dd hh:mm
            ///</summary>
            ///<param name="date" type="string">日期值</param>
            var datepicker = this, options = this.Options;
            var reg = /^(\d{4})(-|\/)(\d{2})\2(\d{2}) (\d{2}):(\d{2})$/;
            var r = date.match(reg);
            if (r == null) return false;
            var d = new Date(r[1], r[3] - 1, r[4], r[5], r[6]);
            if (d == "NaN") return false;
            return (d.getFullYear() == r[1] && (d.getMonth() + 1) == r[3] && d.getDate() == r[4] && d.getHours() == r[5] && d.getMinutes() == r[6]);
        },
        _GetDefaultDate: function () {
            ///<summary>
            /// 获取当前日期值，日期值按照指定格式转换
            ///</summary>
            var datepicker = this, options = this.Options;

            var format = options.DateFormat;
            if (options.ShowType == "ymd" && !options.CanShowTime) {
                format = options.DateFormat.substring(0, 10);
            }
            else if (options.ShowType == "ym") {
                format = options.DateFormat.substring(0, 7);
            }
            options.DateFormat = format;
            var value = datepicker.Now.ToDateTime().Format(format);

            if (datepicker.HasBind("Extractor"))
                value = datepicker.Trigger("Extractor", [value]);

            return value;
        },
        _SetDate: function () {
            ///<summary>
            /// 设置文本框值
            ///</summary>
            var datepicker = this, options = this.Options;
            var value = datepicker._GetDefaultDate();

            datepicker._SetValue(value);
        },
        _SetValue: function (value) {
            ///<summary>
            /// 设置文本框显示的日期值
            ///</summary>
            ///<param name="value" type="string">日期值</param>
            var datepicker = this, options = this.Options;

            if (value == "")
                return;

            var val = datepicker._Resolve(value);

            // 若传入参数的日期值经过验证后失败，则val为null，使用当前日期默认值进行转换赋值
            value = ((val == null) ? datepicker._GetDefaultDate() : new Date(val).Format(options.DateFormat));

            // 这里进行进一步解析处理的条件必须value的值不是通过调用_GetDefaultDate()方法得到，
            // 因为_GetDefaultDate()方法中已调用该事件
            if (datepicker.HasBind("Extractor") && val != null)
                value = datepicker.Trigger("Extractor", [value]);

            $(datepicker.Element).val(value);
            $(datepicker.Element).trigger("change");
        },
        _GetValue: function () {
            var datebox = this, options = this.Options;
            return $(datebox.Element).val();
        },
        _Resolve: function (dateString) {
            var datepicker = this, options = this.Options;
            var val = null;

            // 日期格式验证
            //-----------------------------------------------------------------
            // 若配置不显示时间，即表示日期格式为短日期格式
            if (datepicker.HasBind("Resolve"))
                dateString = datepicker.Trigger("Resolve", [dateString]);

            if (!options.CanShowTime && datepicker._IsDateTime(dateString))
                val = dateString.replace(/-/g, "/");
            // 若配置显示时间，即表示日期格式为长日期格式
            else if (options.CanShowTime && datepicker._IsLongDateTime(dateString))
                val = dateString.replace(/-/g, "/");


            if (val != null) {
                var date = new Date(val);
                datepicker.Now.Year = date.getFullYear().toString();
                datepicker.Now.Month = (date.getMonth() + 1) < 10 ? ("0" + (date.getMonth() + 1)) : (date.getMonth() + 1).toString();
                datepicker.Now.Day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate().toString();
                if (options.CanShowTime) {
                    datepicker.Now.Hour = date.getHours() < 10 ? "0" + date.getHours() : date.getHours().toString();
                    datepicker.Now.Minute = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes().toString();
                    $("label.for-hour", datepicker.Calendar.TimeContainer).text(datepicker.Now.Hour);
                    $("label.for-minute", datepicker.Calendar.TimeContainer).text(datepicker.Now.Minute);
                }
            }

            return val;
        },
        _SetIsShowDate: function (value) {
            var datepicker = this, options = this.Options;
            var date = datepicker._GetDefaultDate();
            if (value) {
                $(datepicker.Element).val(date);
            }
        },
        _SetDisabled: function (value) {
            var datepicker = this, options = this.Options;
            datepicker.Disabled = value;
            if (value) {
                $(datepicker.Element).attr("disabled", value);
            }
            else {
                $(datepicker.Element).removeAttr("disabled");
            }
        },
        _GetDisabled: function () {
            var datepicker = this, options = this.Options;
            return datepicker.Disabled;
        },
        _SetReadOnly: function (value) {
            ///<summary>
            /// 设置文本框是否为只读
            ///</summary>
            ///<param name="value" type="bool">设置一个值，该值标识文本框是否为只读。</param>
            var datepicker = this, options = this.Options;
            if (value != null) {
                if (value)
                    $(datepicker.Element).attr("readonly", "readonly");
                else
                    $(datepicker.Element).removeAttr("readonly");
            }
        },
        _GetReadOnly: function () {
            var datepicker = this, options = this.Options;
            return $(datepicker.Element).attr("readonly");
        },
        SetValue: function (value) {
            var datepicker = this, options = this.Options;
            datepicker._SetValue(value);
        },
        GetValue: function () {
            var datepicker = this, options = this.Options;
            return datepicker._GetValue();
        },
        Disable: function () {
            var datepicker = this, options = this.Options;
            datepicker.Set({ Disabled: true });
        },
        Enable: function () {
            var datepicker = this, options = this.Options;
            datepicker.Set({ Disabled: false });

        }
    });

})(jQuery);