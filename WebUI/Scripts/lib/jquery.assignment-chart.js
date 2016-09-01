/**
 * widget for owner grid on owner page frontend
 */
; (function ($, window, document, undefined) {
    $.widget('karental.assignmentChart', {
        options: {
            carModelName: [], //{ text: "FREED", value: "0f4dfe79-9264-4bbc-9ae6-2ee173b19522", color: "#6eb3fa" }
            licensePlate: [], //{ text: "D 2 A", value: "D 2 A", color: "#51a0ed" }
            bindingUrl: '',
            editUrl: '',
        },

        //attributes
        dataSource: null,

        _create: function () {
            var self = this;

            //datasource
            self.dataSource = new kendo.data.SchedulerDataSource({
                batch: true,
                transport: {
                    read: {
                        url: self.options.bindingUrl,
                        dataType: "json"
                    },
                    parameterMap: function (options, operation) {
                        if (operation !== "read" && options.models) {

                        } else {
                            var scheduler = $(self.element).data('kendoScheduler');
                            var start = scheduler.view().startDate();
                            var end = scheduler.view().endDate();

                            filter = options.filter.filters;
                            filter.push({ field: 'StartDate', operator: 'eq', value: kendo.toString(start, "yyyy-MM-dd") });
                            filter.push({ field: 'EndDate', operator: 'eq', value: kendo.toString(end, "yyyy-MM-dd") });

                            return options;
                        }
                    }
                },
                schema: {
                    model: {
                        id: "Id",
                        startTimezone: null,
                        endTimezone: null,
                        recurrenceId: null,
                        recurrenceRule: null,
                        recurrenceException: null,
                        isAllDay: false,
                        fields: {
                            Id: { from: "Id", type: "string" },
                            title: { from: "Code", defaultValue: "No title" },
                            start: { type: "date", from: "StartRent" },
                            end: { type: "date", from: "FinishRent" },
                            description: { from: "Status" },
                            CarModelName: { from: "CarModelName", nullable: true },
                            LicensePlate: { from: "LicensePlate" },
                            StatusEnum: { from: "StatusEnum" }
                        }
                    }
                },
                filter: { field: "CarModelName", operator: "eq", value: self.options.carModelName[0].value },
                serverFiltering: true,

            });

            //chart
            self.element.kendoScheduler({
                editable: {
                    template: $("#editor").html(),
                    create: false,
                    destroy: false,
                    confirmation: false,
                    window: {
                        //mengubah title di event
                        title: "Informasi Sewa",
                        selectable: true,

                    }
                },
                //date: new Date("2016/1/1"),
                //startTime: new Date("2016/1/1 07:00 AM"),
                //eventHeight: 10,
                majorTick: 60,
                height: 400,
                eventTemplate: $("#event-template").html(),
                views: [
                    {
                        type: "timelineMonth",
                        //startTime: new Date("2013/6/13 00:00 AM"),
                        majorTick: 1440,
                        columnWidth: 100,
                        //eventHeight: 50
                    }
                ],
                //timezone: "Etc/UTC",
                group: {
                    resources: ["LicensePlate"],
                    orientation: "vertical"
                },
                resources: [
                    {
                        field: "CarModelName",
                        name: "CarModelName",
                        dataSource: self.options.carModelName,
                        title: "CarModelName"
                    },
                    {
                        field: "LicensePlate",
                        name: "LicensePlate",
                        dataSource: self.options.licensePlate,
                        multiple: true,
                        title: "LicensePlate"
                    }
                ],
                dataSource: self.dataSource,
                //fungsi untuk menyembunyikan button save di popup
                //edit: function (e) {
                //    var buttonsContainer = e.container.find(".k-edit-buttons");
                //    var saveButton = buttonsContainer.find(".k-scheduler-update");
                //    saveButton.hide();
                //},
                save: function (e) {
                    console.log(e.event);
                    window.location = self.options.editUrl + '?id=' + e.event.Id;
                },
                messages: {
                    save: 'Ubah'
                },
            });

            self.element.find('.k-scheduler-layout > tbody > tr:first-child .k-scheduler-times tr:first-child th').text(self.options.carModelName[0].text)
        },

        _init: function () { },

        _destroy: function () {
            $.Widget.prototype.destroy.call(this);
        },

        //reload data chart dari binding
        //reload: function (start, end) {
        //    var self = this;

        //    self.dataSource.filter({
        //        filters: [
        //            { field: 'StartDate', operator: 'eq', value: kendo.toString(start, "yyyy-MM-dd") },
        //            { field: 'EndDate', operator: 'eq', value: kendo.toString(end, "yyyy-MM-dd") },
        //            { field: 'CarModelName', operator: 'eq', value: self.options.carModelName },
        //        ],
        //        logic: 'and'
        //    });
        //},

        refresh: function () {
            //console.log('hai');
            var self = this;
            self.dataSource.read();
        },


    });
})(jQuery, window, document);