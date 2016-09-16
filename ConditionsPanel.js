define([
    "//ajax.googleapis.com/ajax/libs/jquery/3.1.0/jquery.min.js"
], function () {
    return function (eventBus, canvas, options) {
        panel = (options.ConditionsPanel == undefined ? null : options.ConditionsPanel);
        options.readonly = (options.readonly == undefined ? false : options.readonly);
        if (panel == null) {
            panel = $('<div></div>');
            width = $($(canvas._container).parent()).width() / 3-9;
            height = $($(canvas._container).parent()).height()-10;
            //#CCCCCC
            panel.attr('style', 'display:none;width:' + width.toString() + 'px;height:' + height.toString() + 'px;position:absolute;right:0;top:0;z-index:20;background-color:#F8F8F8;padding:4px;border-left:solid 1px #CCCCCC;');
            $(canvas._container).after(panel);
        }
        panel.hide();
        eventBus.on('element.click', function (event) {
            var element = event.element;
            var businessObject = element.businessObject;
            if (businessObject != undefined) {
                if (businessObject.$type == 'bpmn:Participant') {
                    businessObject = businessObject.processRef;
                }
            }
            switch (businessObject.$type) {
                case 'bpmn:Process':
                case 'bpmn:StartEvent':
                case 'bpmn:EndEvent':
                case 'bpmn:IntermediateCatchEvent':
                case 'bpmn:IntermediateThrowEvent':
                case 'bpmn:SequenceFlow':
                case 'bpmn:ScriptTask':
                    panel.html('<div style="text-align:center;width:100%;border-bottom:solid 1px #CCCCCC"><h2>' + element.id + '</h2></div>');
                    switch (businessObject.$type) {
                        case 'bpmn:ScriptTask':
                            panel.append('<div style="text-align:center;width:100%;border-bottom:solid 1px #CCCCCC"><h4>Script</h4></div>');
                            panel.append('<div style="text-align:center;width:100%;border-bottom:solid 1px #CCCCCC"><select name="scriptLanguage"><option value="Javascript">Javascript</option><option value="cSharpScript">C# Script</option><option value="VBScript">VB.Net Script</option><textarea cols="35" rows="30" name="scriptCode"></textarea></div>');
                            if (businessObject.extensionElements!=undefined){
                                for(var x=0;x<businessObject.extensionElements.values.length;x++){
                                    if (['exts:cSharpScript','exts:VBScript','exts:Javascript'].indexOf(businessObject.extensionElements.values[x].$type)>=0){
                                        $(panel.find('[name="scriptCode"]')[0]).val(businessObject.extensionElements.values[x].code);
                                        $(panel.find('[name="scriptLanguage"]>option[value="' + businessObject.extensionElements.values[x].$type.substring(4) + '"]')[0]).prop('selected', true);
                                    }
                                }
                            }
                            if (options.readonly) {
                                $(panel.find('[name="scriptCode"],[name="scriptLanguage"]')).prop('enabled', false);
                            }
                            break;
                        case 'bpmn:Process':
                        case 'bpmn:StartEvent':
                        case 'bpmn:EndEvent':
                        case 'bpmn:IntermediateCatchEvent':
                        case 'bpmn:IntermediateThrowEvent':
                        case 'bpmn:SequenceFlow':
                            var dv = $('<div style="text-align:center;width:100%;border-bottom:solid 1px #CCCCCC"><h4>Conditions</h4></div>');
                            panel.append(dv);
                            if (options.readonly) {
                                if (this._RecurBuildConditionMessage==undefined){
                                    this._RecurBuildConditionMessage = function (element) {
                                        var ret = "";
                                        for (var prop in element) {
                                            var val = element[prop];
                                            switch (prop) {
                                                case 'isEqualCondition':
                                                case 'greaterThanCondition':
                                                case 'greaterThanOrEqualCondition':
                                                case 'lessThanCondition':
                                                case 'lessThanOrEqualCondition':
                                                    var left = (val.leftVariable == undefined ? val.left : '${' + val.leftVariable + '}');
                                                    var right = (val.rightVariable == undefined ? val.right : '${' + val.rightVariable + '}');
                                                    var cond = '=';
                                                    switch (prop) {
                                                        case 'greaterThanCondition':
                                                            cond='>';
                                                            break;
                                                        case 'greaterThanOrEqualCondition':
                                                            cond = '>=';
                                                            break;
                                                        case 'lessThanCondition':
                                                            cond = '<';
                                                            break;
                                                        case 'lessThanOrEqualCondition':
                                                            cond = '<=';
                                                            break;
                                                    }
                                                    ret = left + ' '+cond+' ' + right;
                                                    break;
                                                case 'isNull':
                                                    ret = 'Variable["' + val.variable + '"] does not have a value';
                                                    break;
                                                case 'notCondition':
                                                    ret = 'NOT (' + arguments.callee(val) + ')';
                                                    break;
                                                case 'cSharpScript':
                                                case 'Javascript':
                                                case 'VBScript':
                                                    var lang = 'C#';
                                                    switch (prop) {
                                                        case 'Javascript':
                                                            lang = 'JS';
                                                            break;
                                                        case 'VBScript':
                                                            lang = 'VB';
                                                            break;
                                                    }
                                                    ret = lang+'EVAL(' + val.code + ')';
                                                    break;
                                            }
                                        }
                                        return ret;
                                    };
                                }
                                var conditions=null;
                                if (businessObject.extensionElements!=undefined){
                                    for(var x=0;x<businessObject.extensionElements.values.length;x++){
                                        if (businessObject.extensionElements.values[x].$type=="exts:ConditionSet"){
                                            conditions = this._RecurBuildConditionMessage(businessObject.extensionElements.values[x]);
                                        }
                                    }
                                }
                                dv.append('<p>' + (conditions==null ? 'No conditions set' : conditions + '</p>'));
                            }
                            break;
                    }
                    panel.show();
                    break;
            }
        });
    };
});