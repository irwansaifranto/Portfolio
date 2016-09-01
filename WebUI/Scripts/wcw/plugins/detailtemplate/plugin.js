var list = null;
CKEDITOR.plugins.add('detailtemplate',
{
    requires: ['richcombo'], //, 'styles' ],
    init: function (editor) {
        var config = editor.config,
           lang = editor.lang.format;

        // Create style objects for all defined styles.

        editor.ui.addRichCombo('detailtemplate',
           {
               label: "Template",
               title: "Template",
               voiceLabel: "Template",
               className: 'cke_format',
               multiSelect: false,

               panel:
               {
                   css: [config.contentsCss, CKEDITOR.getUrl('skins/moono/' + 'editor.css')],
                   voiceLabel: lang.panelVoiceLabel
               },

               init: function () {
                   this.startGroup("Templates");
                   var returnData = null;
                   $.ajax({
                       url: "/Home/getListTemplate",
                       method: "POST",
                       async: false,
                       data: null,
                   }).done(function (result) { returnData = result.list; });

                   list = returnData;
                   for (var a in returnData) {
                       this.add(a, returnData[a]['template_name'], returnData[a]['template_name']);
                   }
                   //this.add('value', 'drop_text', 'drop_label');
                   //for (var this_tag in tags) {
                   //    this.add(tags[this_tag][0], tags[this_tag][1], tags[this_tag][2]);
                   //}
               },

               onClick: function (value) {
                   editor.focus();
                   editor.fire('saveSnapshot');
                   editor.setData(decodeHtml(list[value]['template_text']));
                   editor.fire('saveSnapshot');
                   // this.setValue(value);
               }
           });
    }
});

function decodeHtml(str) {
    return $('<div/>').html(str).text();
}
