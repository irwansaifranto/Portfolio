(function ($) {
    var BrowserDetect = {
        init: function () {
            this.browser = this.searchString(this.dataBrowser) || "Other";
            this.version = this.searchVersion(navigator.userAgent) || this.searchVersion(navigator.appVersion) || "Unknown";
        },
        searchString: function (data) {
            for (var i = 0; i < data.length; i++) {
                var dataString = data[i].string;
                this.versionSearchString = data[i].subString;

                if (dataString.indexOf(data[i].subString) !== -1) {
                    return data[i].identity;
                }
            }
        },
        searchVersion: function (dataString) {
            var index = dataString.indexOf(this.versionSearchString);
            if (index === -1) {
                return;
            }

            var rv = dataString.indexOf("rv:");
            if (this.versionSearchString === "Trident" && rv !== -1) {
                return parseFloat(dataString.substring(rv + 3));
            } else {
                return parseFloat(dataString.substring(index + this.versionSearchString.length + 1));
            }
        },

        dataBrowser: [
            { string: navigator.userAgent, subString: "Chrome", identity: "Chrome" },
            { string: navigator.userAgent, subString: "MSIE", identity: "Explorer" },
            { string: navigator.userAgent, subString: "Trident", identity: "Explorer" },
            { string: navigator.userAgent, subString: "Firefox", identity: "Firefox" },
            { string: navigator.userAgent, subString: "Safari", identity: "Safari" },
            { string: navigator.userAgent, subString: "Opera", identity: "Opera" }
        ]

    };

    BrowserDetect.init();

    $.fn.emptySelect = function () {
        return this.each(function () {
            if (this.tagName == 'SELECT') this.options.length = 0;
        });
    }

    $.fn.loadSelect = function (optionsDataArray) {
        return this.emptySelect().each(function () {
            if (this.tagName == 'SELECT') {
                var selectElement = this;
//                if (displayBlank) {
//                    selectElement.add(new Option("[Select]", ""), null);
//                }
                $.each(optionsDataArray, function (index, optionData) {
                    var option = new Option(optionData.caption,
                                  optionData.value);
                    if (BrowserDetect.browser == 'Explorer') {
                        selectElement.add(option);
                    }
                    else {
                        selectElement.add(option, null);
                    }
                });
            }
        });
    }

})(jQuery);