/// <reference path="jquery-ui-1.8.20.min.js" />
var kendoGridFilterable = {
    messages: {
        info: "Tampilkan yang memiliki nilai:", // sets the text on top of the filter menu
        filter: "Filter", // sets the text for the "Filter" button
        clear: "Hapus", // sets the text for the "Clear" button

        // when filtering boolean numbers
        isTrue: "Ya", // sets the text for "isTrue" radio button
        isFalse: "Tidak", // sets the text for "isFalse" radio button

        //changes the text of the "And" and "Or" of the filter menu
        and: "Dan",
        or: "Atau"
    },
    operators: {
        //string: {
        //    eq: "Is equal to",
        //    neq: "Is not equal to",
        //    startswith: "Starts with",
        //    contains: "Contains",
        //    endswith: "Ends with"
        //},
        //filter menu for "string" type columns
        string: {
            //eq: "Sama Dengan",
            //neq: "Tidak Sama Dengan",
            //startswith: "Memiliki Awalan",
            contains: "Memiliki Kata",
            //endswith: "Memiliki Akhiran"
        },
        //filter menu for "number" type columns
        number: {
            eq: "Sama Dengan",
            //neq: "Tidak Sama Dengan",
            //gte: "Lebih Besar Atau Sama Dengan",
            //gt: "Lebih Besar",
            //lte: "Lebih Kecil Atau Sama Dengan",
            //lt: "Lebih Kecil"
        },
        //filter menu for "date" type columns
        date: {
            eq: "Sama Dengan",
            //neq: "Tidak Sama Dengan",
            //gte: "Setelah Atau Sama Dengan",
            //gt: "Setelah",
            //lte: "Sebelum Atau Sama Dengan",
            //lt: "Sebelum"
        },
        //filter menu for foreign key values
        enums: {
            eq: "Sama Dengan",
            //neq: "Tidak Sama Dengan"
        }
    },
    extra: false,
};

$(document).ready(function () {
    kendo.culture("id-ID");

    //required sign using asterisk
    $('input[type=text], input[type=hidden], textarea, select, input[type=password]').each(function () {
        var req = $(this).attr('data-val-required');
        if (undefined != req) {
            var label = $(this).parentsUntil('form').find('label[for="' + $(this).attr('id') + '"]');
            var text = label.text();
            if (text.length > 0) {
                label.append('<span style="color:red"> *</span>');
            }
        }
    });

    $.ajaxSetup({ cache: false }); //disable cache for IE

    jQuery.fn.outerHTML = function () {
        return (this[0]) ? this[0].outerHTML : '';
    };

    //tooltip untuk input di form
    $("input[type=text]").tooltip({
        trigger: "focus",
        title: function () {
            var title = "";
            var titleArray = [];

            if ($(this).attr("data-val-required") != null)
                titleArray[titleArray.length] = $(this).attr("data-val-required");

            if ($(this).attr("data-val-number") != null)
                titleArray[titleArray.length] = $(this).attr("data-val-number");

            title = titleArray.join(" ");

            return title;
        },
    });

    //datepicker format
    $('.form-control-datepicker').kendoDatePicker({
        format: "yyyy-MM-dd",
        parseFormats: ["dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH.mm.ss"]
    }).prop('readonly', 'readonly');

    $('.form-control-datetimepicker').kendoDateTimePicker({
        format: 'yyyy-MM-dd HH:mm',
        parseFormats: ["dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH.mm.ss"],
        interval: 30
    }).prop('readonly', 'readonly');

    //konfirmasi ketika user close window tapi blm save
    var formChange = false;
    $("form").each(function () {
        if (!$(this).hasClass('not-track-change')) {
            $(this).find("input, select, textarea").change(function () {
                formChange = true;
            });
            $(this).submit(function () {
                $(this).find('button').attr('disabled', true);
                formChange = false;
            });
        }
    });
    $(window).on("beforeunload", function () {
        if (formChange) {
            return "Perubahan data belum tersimpan.";
        }
    });
    $('form button').removeAttr('disabled');

    //CKEditor
    if (typeof CKEDITOR !== "undefined") {
        CKEDITOR.config.toolbar = [
            //['Styles', 'Format', 'Font', 'FontSize'],
            ['Bold', 'Italic', 'Underline', 'StrikeThrough', '-', 'Outdent', 'Indent', '-'],
            ['NumberedList', 'BulletedList', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
            //['Image', 'Table', '-', 'Link', 'Flash', 'Smiley', 'TextColor', 'BGColor', 'Source']
        ];

        if ($('.ckeditor-image-browser').length > 0) {
            CKEDITOR.config.toolbar.push(['Image']);
            CKEDITOR.config.filebrowserImageBrowseUrl = FILEMANAGERPATH;
        }
    }

    //konfirmasi delete button
    $('.delete-button').click(function (e) {
        return confirm("Anda yakin menghapus data?");
    });

    //tooltip untuk help
    $('.project-help .glyphicon').each(function () {
        $(this).tooltip({
            trigger: "hover",
            title: function () {
                return $(this).attr('title');
            },
        });
    });

    //numeric textbox
    $('.form-control-numeric').kendoNumericTextBox({
        min: 0,
        max: 1000000000000000000,
        decimals: 0,
        format: 'n0',
    });
    $('.form-control-decimal').kendoNumericTextBox({
        min: 0,
        max: 1000000000000000000,
        decimals: 2,
        format: 'n2',
    });

    //kendo textarea
    $('.form-control-editor').kendoEditor({
        tools: [
            { name: "insertLineBreak", shift: false },
            { name: "insertParagraph", shift: true }
        ],
        encoded: false,
        paste: function (ev) {
            ev.html = $(ev.html).text();
        }
    });

    $('#global-setting-form select').change(function () {
        var settingForm = $('#global-setting-form');
        settingForm.attr('action', settingForm.attr('action') + '?returnUrl=' + window.location.href);

        this.form.submit();
    });
});

/**
 * parse xml date
 * 2013-12-04T18:10:05.768
 */
function TimeStampToDate(xmlDate) {
    var dt = new Date();
    var dtS = xmlDate.slice(xmlDate.indexOf('T') + 1, xmlDate.indexOf('.'))
    var TimeArray = dtS.split(":");
    //console.log(TimeArray);
    dt.setHours(TimeArray[0]);
    dt.setMinutes(TimeArray[1]);
    dtS = xmlDate.slice(0, xmlDate.indexOf('T'))
    TimeArray = dtS.split("-");
    //console.log(TimeArray);
    dt.setFullYear(TimeArray[0]);
    dt.setMonth(TimeArray[1] - 1);
    dt.setDate(TimeArray[2]);
    //console.log(dt);
    return dt;
}

/**
 * mengambil tanggal dari datetime
 * @param datetime 12/31/2013 12:00:00 AM
 * @return 12/31/2013
 */
function GetDate(datetime) {
    var parts = datetime.split(' ');
    var date = parts[0];

    return date;
}

/**
 * mengambil jam dari datetime
 * @param datetime 12/31/2013 12:00:00 AM
 * @return 12:00
 */
function GetTime(datetime) {
    //kamus
    var parts = datetime.split(' ');
    var ampm = parts[2];
    var dateObject = new Date(datetime);

    //hour & minute
    var hour = dateObject.getHours();
    var minute = dateObject.getMinutes();
    if (ampm == 'PM')
        hour += 12;
    hour = String("00" + hour).slice(-2);
    minute = String("00" + minute).slice(-2);

    //time string
    var time = hour + ':' + minute;

    return time;
}

/**
 * fungsi desimal
 */
function ThousandSeparator(n, sep) {
    var sRegExp = new RegExp('(-?[0-9]+)([0-9]{3})'), sValue = n + '';
    if (sep === undefined) { sep = ','; }
    while (sRegExp.test(sValue)) {
        sValue = sValue.replace(sRegExp, '$1' + sep + '$2');
    }
    return sValue;
}

/**
 * html decode
 * untuk web grid bisa menggunakan atribut encoded di kolom
 */
function FormatText(text) {
    var textContainer = $("<div></div>");
    if (text != null) {
        textContainer.html(text.replace(/\n/g, "<br>"));
    }
    return textContainer.html();
}

/**
 * menghapus nilai dari $(selector)
 */
function clearValue(selector) {
    $(selector).val("");
}

//KENDO GRID

/**
 * menampilkan info data kosong
 * @param el: jquery element
 * @param message: pesan
 */
function displayEmptyGrid(el, message) {
    el.removeClass();
    el.attr("style", "");
    el.html(message);
}

//dropdown filter interval
function typeFilterDatepicker(element) {
    //element.kendoDatePicker({
    //    format: 'yyyy-MM-dd',
    //    parseFormats: ["yyyy-MM-dd"],
    //    culture: 'id-ID',
    //});
    element.datepicker({
        dateFormat: "yy-mm-dd",
        //onSelect: function() {
        //$(this).data('datepicker').inline = true;
        //element.click();
        //$('.k-grid-filter').first().click();
        //},
        //onClose: function() {
        //    $(this).data('datepicker').inline = false;
        //}
    }).addClass("k-textbox").attr("readonly", "readonly");
}

/**
 * kendo grid mengambil data dari row
 */
function getDataRowGrid(e) {
    return $(e.target).closest("tr");
}

/**
 * konfirmasi delete sebelum di redirect
 * response: { Success: t/f, Message }
 */
function goToDeletePage(url, datasource) {
    swal(
        {
            title: "Hapus Data",
            text: "Apakah anda yakin untuk menghapus data ini?",
            type: "warning",
            cancelButtonText: "Batal",
            showCancelButton: true,
            confirmButtonClass: "btn btn-primary",
            confirmButtonText: "Ya",
            closeOnConfirm: false
        },
        function () {
            swal({
                title: "Loading",
                text: "Harap Menunggu...",
                imageUrl: "/Content/sweet-alert/ajax-loader.gif",
                closeOnConfirm: false,
                confirmButtonClass: "hidden",
                //imageSize: "80x80"
            });

            $.ajax({
                url: url,
                type:"POST",
                success: function (data) {
                    if (data.Success == true) {
                        if (datasource != null) {
                            datasource.read();
                        }
                        swal("Status", "Data berhasil dihapus", "success");
                    }
                    else {
                        swal(data.Message);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    swal("Server Error. Harap hubungi administrator");
                }
            });        
        });
}

//delete via post
function postToDeletePage(url, data, datasource) {
    swal(
        {
            title: "Hapus Data",
            text: "Apakah anda yakin untuk menghapus data ini?",
            type: "warning",
            cancelButtonText: "Batal",
            showCancelButton: true,
            confirmButtonClass: "btn btn-primary",
            confirmButtonText: "Ya",
            closeOnConfirm: false
        },
        function () {
            swal({
                title: "Loading",
                text: "Harap Menunggu...",
                imageUrl: "/Content/sweet-alert/ajax-loader.gif",
                closeOnConfirm: false,
                confirmButtonClass: "hidden",
            });

            $.ajax({
                url: url,
                type: "POST",
                data: data,
                success: function (response) {
                    if (response.Success == true) {
                        if (datasource != null) {
                            datasource.read();
                        }
                        swal("Status", "Data berhasil dihapus", "success");
                    }
                    else {
                        swal(response.Message);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    swal("Server Error. Harap hubungi administrator");
                }
            });
        }
    );
}

function deleteThenRedirect(url, data, redirectPage) {
    swal(
        {
            title: "Hapus Data",
            text: "Apakah anda yakin untuk menghapus data ini?",
            type: "warning",
            cancelButtonText: "Batal",
            showCancelButton: true,
            confirmButtonClass: "btn btn-primary",
            confirmButtonText: "Ya",
            closeOnConfirm: false
        },
        function () {
            swal({
                title: "Loading",
                text: "Harap Menunggu...",
                imageUrl: "/Content/sweet-alert/ajax-loader.gif",
                closeOnConfirm: false,
                confirmButtonClass: "hidden",
            });

            $.ajax({
                url: url,
                type: "POST",
                data: data,
                success: function (response) {
                    if (response.Success == true) {
                        window.location.href = redirectPage;
                    }
                    else {
                        swal(response.Message);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    swal("Server Error. Harap hubungi administrator");
                }
            });
        }
    );
}

/**
 * disable $(el) dengan menambahkan kelas disabled
 */
function disableMe(el) {
    $(el).addClass("disabled");
}

/**
 * mengecek kalau a == null menampilkan b
 */
function isnull(a, b) {
    b = b || '';
    return a || b;
}

/**
 * decode hasil penyimpanan dari kendo grid
 */
function htmlDecode(value) {
    var result = '';
    if (value != null) {
        result = value.replace(/&lt;/g, "<").replace(/&gt;/g, ">");
    }

    return result;
}

function comboBoxOnChange(e) {
    if (this.value() && this.selectedIndex == -1) {   //or use this.dataItem to see if it has an object
        this.text('');
    }
}

function openPrintWindow(url, windowName) {
    window.open(url, windowName, "height=800,width=793,modal=yes,alwaysRaised=yes");
}

function getFileType(filename) {
    return filename.substr(filename.lastIndexOf('.') + 1)
}