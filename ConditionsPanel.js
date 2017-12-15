define([
    "//ajax.googleapis.com/ajax/libs/jquery/3.1.0/jquery.min.js"
], function () {
    return function (eventBus, canvas, moddle, options) {
        var formGenerator = {
            compareCondition: function (val) {
                var ret = $('<div></div>');
                ret.append('Left: <input type="text" name="left"/><br/>');
                ret.append('Comparison: <select name="comparison"><option value="isEqualCondition">=</option><option value="greaterThanCondition">&gt;</option><option value="greaterThanOrEqualCondition">&gt;=</option><option value="lessThanCondition">&lt;</option><option value="lessThanOrEqualCondition">&lt;=</option></select><br/>');
                ret.append('Right: <input type="text" name="right"/><br/>');
                ret.append('Negated: <input type="checkbox" name="negated"/><br/>');
                $(ret.find('[name="left"]')).val((val.leftVariable == undefined ? val.left : '${' + val.leftVariable + '}'));
                $(ret.find('option[value="' + val.$type.substring(5) + '"]')[0]).prop('selected', true);
                $(ret.find('[name="right"]')).val((val.rightVariable == undefined ? val.right : '${' + val.rightVariable + '}'));
                $(ret.find('[name="negated"]')).prop('checked', val.negated);
                return ret;
            },
            containsCondition:function(val){
                ret.append('Variable: <input type="text" name="leftVariable"/><br/>');
                ret.append('Contains: <input type="text" name="right"/><br/>');
                ret.append('Negated: <input type="checkbox" name="negated"/><br/>');
                $(ret.find('[name="leftVariable"]')).val(val.leftVariable);
                $(ret.find('[name="right"]')).val((val.rightVariable == undefined ? val.right : '${' + val.rightVariable + '}'));
                $(ret.find('[name="negated"]')).prop('checked', val.negated);
                return ret;
            },
            isnull: function (val) {
                var ret = $('<div></div>');
                ret.append('Variable: <input type="text" name="variable"/><br/>');
                ret.append('Negated: <input type="checkbox" name="negated"/><br/>');
                $(ret.find('[name="variable"]')).val(val.variable);
                $(ret.find('[name="negated"]')).prop('checked', val.negated);
                return ret;
            },
            script: function (val) {
                var ret = $('<div></div>');
                ret.append('<div style="text-align:center;width:100%;border-bottom:solid 1px #CCCCCC"><select name="scriptLanguage"><option value="Javascript">Javascript</option><option value="cSharpScript">C# Script</option><option value="VBScript">VB.Net Script</option><textarea cols="35" rows="30" name="scriptCode"></textarea></div>');
                $(ret.find('option[value="'+val.$type.sub(5)+'"]')).prop('selected',true);
                $(ret.find('textarea')[0]).val(val.code);
                return ret;
            },
            groupCondition: function (val) {
                var ret = $('<div></div>');
                ret.append('Group Type: <select><option value="AND">AND</option><option value="OR">OR</option></select><br/>');
                ret.append('Negated: <input type="checkbox" name="negated"/><br/>');
                switch (val.$type) {
                    case 'exts:orCondition':
                        $(ret.find('option[value="OR"]')[0]).prop('selected', true);
                        break;
                    case 'exts:andCondition':
                        $(ret.find('option[value="AND"]')[0]).prop('selected', true);
                        break;
                }
                $(ret.find('[name="negated"]')).prop('checked', val.negated);
                return ret;
            }
        };
        var contentUpdater = {
            compareCondition: function (frm,elem) {
                $(elem.find('[name="left"]')[0]).html($(frm.find('[name="left"]')).val());
                $(elem.find('[name="right"]')[0]).html($(frm.find('[name="right"]')).val());
                if (frm.find('input[name="negated"]:checked').length == 0) {
                    $(elem.find('[name="negated"]')).show();
                } else {
                    $(elem.find('[name="negated"]')).hide();
                }
                switch ($(frm.find('select[name="comparison"]>option:selected')[0]).val()) {
                    case 'isEqualCondition':
                        $(elem.find('[name="comparison"]')[0]).html('=');
                        break;
                    case 'greaterThanCondition':
                        $(elem.find('[name="comparison"]')[0]).html('>');
                        break;
                    case 'greaterThanOrEqualCondition':
                        $(elem.find('[name="comparison"]')[0]).html('>=');
                        break;
                    case 'lessThanCondition':
                        $(elem.find('[name="comparison"]')[0]).html('<');
                        break;
                    case 'lessThanOrEqualCondition':
                        $(elem.find('[name="comparison"]')[0]).html('<=');
                        break;
                }
            },
            containsCondition:function(frm,elem){
                $(elem.find('[name="leftVariable"]')[0]).html($(frm.find('[name="leftVariable"]')).val());
                $(elem.find('[name="right"]')[0]).html($(frm.find('[name="right"]')).val());
                if (frm.find('input[name="negated"]:checked').length == 0) {
                    $(elem.find('[name="negated"]')).show();
                } else {
                    $(elem.find('[name="negated"]')).hide();
                }
            },
            isnull: function (frm,elem) {
                $(elem.find('[name="variable"]')[0]).html($(frm.find('[name="variable"]')[0]).val());
                if (frm.find('input[name="negated"]:checked').length == 0) {
                    $(elem.find('[name="negated"]')).show();
                } else {
                    $(elem.find('[name="negated"]')).hide();
                }
            },
            script: function (frm,elem) {
                $(elem.find('[name="code"]')[0]).html($(frm.find('textarea')[0]).val());
                switch ($(frm.find('select[name="scriptLanguage"]>option:selected')[0]).val()) {
                    case 'cSharpScript':
                        $(elem.find('[name="language"]')[0]).html('C#');
                        break;
                    case 'javascript':
                        $(elem.find('[name="language"]')[0]).html('JS');
                        break;
                    case 'VBScript':
                        $(elem.find('[name="language"]')[0]).html('VB');
                        break;
                }
            },
            groupCondition: function (frm, elem) {
                if (frm.find('input[name="negated"]:checked').length == 0) {
                    $(elem.find('[name="negated"]')).show();
                } else {
                    $(elem.find('[name="negated"]')).hide();
                }
                $(elem.find('[name="condition"]')[0]).html($(frm.find('option:selected')[0]).val());
            }
        };
        var contentParser = {
            _randomIndex:function(){
                return Math.floor(Math.random() * 5000);
            },
            _base:function(parentId,index){
                return tmp = { id: (parentId==undefined ? '' : parentId) + '_cond' + ((index==undefined ? this._randomIndex() : index)+1).toString() };
            },
            compareCondition:function(elem,parentId,index){
                var tmp = this._base(parentId, index);
                tmp.negated = elem.find('[name="negated"]:visible').length > 0;
                var ret=null;
                switch ($(elem.find('[name="comparison"]:first')[0]).text()) {
                    case '=':
                        ret = moddle.create('exts:isEqualCondition', tmp);
                        break;
                    case '>':
                        ret = moddle.create('exts:greaterThanCondition', tmp);
                        break;
                    case '>=':
                        ret = moddle.create('exts:greaterThanOrEqualCondition', tmp);
                        break;
                    case '<':
                        ret = moddle.create('exts:lessThanCondition', tmp);
                        break;
                    case '<=':
                        ret = moddle.create('exts:lessThanOrEqualCondition', tmp);
                        break;
                }
                ['left', 'right'].forEach(function (e) {
                    var val = $(elem.find('[name="' + e + '"]:first')[0]).text();
                    if (val.indexOf('${') >= 0) {
                        ret.set(e + 'Variable',val.substring(2, val.length - 1));
                    } else {
                        ret.set(e,val);
                    }
                });
                return ret;
            },
            containsCondition:function(elem,parentId,index){
                var tmp = this._base(parentId, index);
                tmp.negated = elem.find('[name="negated"]:visible').length > 0;
                tmp.leftVariable = $(elem.find('[name="leftVariable"]:first')[0]).text();
                var val = $(elem.find('[name="right"]:first')[0]).text();
                if (val.indexOf('${')>=0){
                    tmp.rightVariable = val.substring(2,val.length-1);
                }else{
                    tmp.right = val;
                }
                return moddle.create('exts:containsCondition', tmp);
            },
            isnull : function(elem,parentId,index){
                var tmp = this._base(parentId, index);
                tmp.negated = elem.find('[name="negated"]:visible').length > 0;
                var ret = moddle.create('exts:isNull', tmp);
                ret.set('variable',$(elem.find('[name="variable"]:first')[0]).text());
                return ret;
            },
            script:function(elem,parentId,index){
                var tmp = this._base(parentId,index);
                var ret=null;
                switch ($(elem.find('[name="language"]:first')[0]).text()) {
                    case 'C#':
                        ret = moddle.create('exts:CSharpScript', tmp);
                        break;
                    case 'JS':
                        ret = moddle.create('exts:Javascript', tmp);
                        break;
                    case 'VB':
                        ret = moddle.create('exts:VBScript', tmp);
                        break;
                }
                ret.set('code',$(elem.find('[name="code"]:first')[0]).text());
                return ret;
            },
            groupCondition:function(elem,parentId,index){
                var tmp = this._base(parentId, index);
                tmp.negated = elem.find('[name="negated"]:visible').length > 0;
                var ret=null;
                switch($(elem.find('[name="condition"]:first')[0]).text()){
                    case 'OR':
                        ret = moddle.create('exts:orCondition', tmp);
                        break;
                    case 'AND':
                        ret = moddle.create('exts:andCondition', tmp);
                        break;
                }
                var lis = $(elem.find('ul:first')[0]).children();
                for (var x = 0; x < lis.length; x++) {
                    if ($(lis[x]).children().length != 0) {
                        if (index != undefined) {
                            index++;
                        }
                        var child = $($(lis[x]).children()[0]);
                        var sub = this[child.attr('name')](child, parentId, index);
                        if (sub != null) {
                            if (ret.get(sub.$type.substring(5)) == undefined) {
                                ret.set(sub.$type.substring(5), [sub]);
                            } else {
                                ret.get(sub.$type.substring(5)).push(sub);
                            }
                        }
                    }
                }
                return ret;
            }
        };
        var renderer = {
            _base:function(val){
                var ret = $('<span style="display:block;clear:right;"></span>');
                if (val.id != undefined) {
                    ret.attr('id', val.id);
                }
                return ret;
            },
            _createDelete: function () {
                var but = $('<button name="delete" style="float:right;">Delete</button>');
                but.on('click', function (event) {
                    $($(event.target).parent()).remove();
                });
                return but;
            },
            _createEdit:function(){
                var but = $('<button name="edit" style="float:right;">Edit</button>');
                but.on('click', function (event) {
                    var elem = $($(event.target).parent());
                    var cont = $($(event.target).parents('[name="condition_container"]')[0]);
                    cont.hide();
                    var frm = formGenerator[elem.attr('name')](contentParser[elem.attr('name')](elem));
                    var butCancel = $('<button>Cancel</button>');
                    var butOkay = $('<button>Okay</button>');
                    frm.append(butOkay);
                    butOkay.on('click', function () {
                        frm.hide();
                        contentUpdater[elem.attr('name')](frm,elem);
                        frm.remove();
                        cont.show();
                    });
                    frm.append(butCancel);
                    butCancel.on('click', { frm: frm }, function (event) {
                        $(event.data.frm.prev()).show();
                        event.data.frm.remove();
                    });
                    cont.after(frm);
                });
                return but;
            },
            _createAdd:function(){
                var but = $('<button name="add" style="float:right;">Add</button>');
                but.on('click', function (event) {
                    var elem = $($($(event.target).parent()).parent());
                    var cont = $($(event.target).parents('[name="condition_container"]')[0]);
                    cont.hide();
                    var frm = $('<div></div>');
                    frm.html('<select><option value="compareCondition">Comparison</option><option value="containsCondition">Contains</option><option value="isnull">Does not have value</value><option value="script">Script</option><option value="groupCondition">AND/OR</option></select><div></div>');
                    var subfrm = $(frm.find('div')[0]);
                    subfrm.append(formGenerator.compareCondition({
                        '$type': 'exts:NA',
                        left: '',
                        right: ''
                    }));
                    $(frm.find('select')[0]).change(function () {
                        var val = $(frm.find('select>option:selected')[0]).val();
                        subfrm.html('');
                        var cfrm = formGenerator[val]({
                            '$type': 'exts:NA',
                            left: '',
                            right: '',
                            variable: '',
                            code: ''
                        });
                        subfrm.append(cfrm);
                    });
                    var butCancel = $('<button>Cancel</button>');
                    var butOkay = $('<button>Okay</button>');
                    frm.append(butOkay);
                    butOkay.on('click', function () {
                        frm.hide();
                        var li = $('<li></li>');
                        elem.append(li);
                        switch ($(frm.find('select>option:selected')[0]).val()) {
                            case 'compareCondition':
                                li.append(renderer.isEqualCondition({
                                    leftVariable: '',
                                    rightVariable: ''
                                },true));
                                break;
                            case 'containsCondition':
                                li.append(renderer.containsCondition({
                                    leftVariable: '',
                                    right:''
                                },true));
                            case 'isnull':
                                li.append(renderer.isNull({ variable: '' }, true));
                                break;
                            case 'script':
                                li.append(renderer.cSharpScript({ code: '' }, true));
                                break;
                            case 'groupCondition':
                                li.append(renderer.orCondition({}, true));
                                break;
                        }
                        contentUpdater[$(frm.find('select>option:selected')[0]).val()](subfrm, li);
                        frm.remove();
                        cont.show();
                    });
                    frm.append(butCancel);
                    butCancel.on('click', { frm: frm }, function (event) {
                        $(event.data.frm.prev()).show();
                        event.data.frm.remove();
                    });
                    cont.after(frm);
                });
                return but;
            },
            isEqualCondition : function(val,readonly){
                var ret = this._base(val);
                ret.attr('name', 'compareCondition');
                var left = (val.leftVariable == undefined ? val.left : '${' + val.leftVariable + '}');
                var right = (val.rightVariable == undefined ? val.right : '${' + val.rightVariable + '}');
                var cond = '=';
                ret.append('<span name="negated">NOT (</span><span name="left">' + left + '</span> <span name="comparison">' + cond + '</span> <span name="right">' + right + '</span><span name="negated">(</span>');
                if (!val.negated) { $(ret.find('[name="negated"]')).hide();}
                if (!readonly) {
                    ret.append(this._createDelete());
                    ret.append(this._createEdit());
                }
                return ret;
            },
            greaterThanCondition : function(val,readonly){
                var ret = this.isEqualCondition(val,readonly);
                $(ret.find('[name="comparison"]')[0]).html('>');
                return ret;
            },
            greaterThanOrEqualCondition : function(val,readonly){
                var ret = this.isEqualCondition(val,readonly);
                $(ret.find('[name="comparison"]')[0]).html('>=');
                return ret;
            },
            lessThanCondition : function(val,readonly){
                var ret = this.isEqualCondition(val,readonly);
                $(ret.find('[name="comparison"]')[0]).html('<');
                return ret;
            },
            lessThanOrEqualCondition : function(val,readonly){
                var ret = this.isEqualCondition(val,readonly);
                $(ret.find('[name="comparison"]')[0]).html('<=');
                return ret;
            },
            containsCondition:function(val,readonly){
                var ret = this._base(val);
                ret.attr('name', 'containsCondition');
                ret.append('<span name="negated">NOT (</span><span name="leftVariable">' + val.leftVariable + '</span> contains <span name="right">'+(val.rightVariable==undefined ? val.right : '${'+val.rightVariable+'}')+'</span><span name="negated">(</span>');
                if (!val.negated) { $(ret.find('[name="negated"]')).hide(); }
                if (!readonly) {
                    ret.append(this._createDelete());
                    ret.append(this._createEdit());
                }
                return ret;
            },
            isNull : function(val,readonly){
                var ret = this._base(val);
                ret.attr('name', 'isnull');
                ret.append('<span name="negated">NOT (</span>Variable["<span name="variable">' + val.variable + '</span>"] does not have a value<span name="negated">(</span>');
                if (!val.negated) { $(ret.find('[name="negated"]')).hide(); }
                if (!readonly) {
                    ret.append(this._createDelete());
                    ret.append(this._createEdit());
                }
                return ret;
            },
            cSharpScript:function(val,readonly){
                var ret = this._base(val);
                ret.attr('name', 'script');
                var lang = 'C#';
                ret.append('<span name="language">' + lang + '</span>EVAL(<span name="code">' + val.code + '</span>)');
                if (!readonly) {
                    ret.append(this._createDelete());
                    ret.append(this._createEdit());
                }
            },
            Javascript:function(val,readonly){
                var ret = this.cSharpScript(val,readonly);
                $(ret.find('[name="language"]')[0]).html('JS');
                return ret;
            },
            VBScript:function(val,readonly){
                var ret = this.cSharpScript(val,readonly);
                $(ret.find('[name="language"]')[0]).html('VB');
                return ret;
            },
            andCondition:function(val,readonly){
                var ret = this._base(val);
                ret.attr('name', 'groupCondition');
                ret.append('<span name="negated">NOT (</span><span name="condition" style="text-align:left;float:left;">AND</span><span name="negated">(</span>');
                var tmp = $('<ul style="list-style:none;padding:0;padding-left:5px;border-left:solid 1px black;text-align:left;margin:0;"></ul>');
                if (!val.negated) { $(ret.find('[name="negated"]')).hide(); }
                for (var p in val) {
                    if (this[p]!=undefined){
                        var sval = val[p];
                        if (!Array.isArray(sval)) {
                            sval = [sval];
                        }
                        for (var x = 0; x < sval.length; x++) {
                            var li = $('<li style="padding-left:5px;"></li>');
                            li.append(this[p](sval[x],readonly));
                            tmp.append(li);
                        }
                    }
                }
                if (!readonly) {
                    ret.append(this._createDelete());
                    ret.append(this._createEdit());
                    ret.append(this._createAdd());
                }
                ret.append('<br/>');
                ret.append(tmp);
                return ret;
            },
            orCondition:function(val,readonly){
                var ret = this.andCondition(val,readonly);
                $(ret.find('[name="condition"]')[0]).html('OR');
                return ret;
            }
        };

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
                            var index = null;
                            panel.append('<div style="text-align:center;width:100%;border-bottom:solid 1px #CCCCCC"><h4>Script</h4></div>');
                            panel.append('<div style="text-align:center;width:100%;border-bottom:solid 1px #CCCCCC"><select name="scriptLanguage"><option value="Javascript">Javascript</option><option value="cSharpScript">C# Script</option><option value="VBScript">VB.Net Script</option><textarea cols="35" rows="30" name="scriptCode"></textarea></div>');
                            if (businessObject.extensionElements != undefined) {
                                for(var x=0;x<businessObject.extensionElements.values.length;x++){
                                    if (['exts:CSharpScript', 'exts:VBScript', 'exts:Javascript'].indexOf(businessObject.extensionElements.values[x].$type) >= 0) {
                                        $(panel.find('[name="scriptCode"]')[0]).val(businessObject.extensionElements.values[x].code);
                                        $(panel.find('[name="scriptLanguage"]>option[value="' + businessObject.extensionElements.values[x].$type.substring(4) + '"]')[0]).prop('selected', true);
                                        index = x;
                                        break;
                                    }
                                }
                            }
                            if (options.readonly) {
                                $(panel.find('[name="scriptCode"],[name="scriptLanguage"]')).prop('disabled', true);
                            } else {
                                var butOk = $('<button>Okay</button>');
                                var butCancel = $('<button>Cancel</button>');
                                var butDiv = $('<div style="text-align:center;width:100%;border-bottom:solid 1px #CCCCCC"></div>');
                                butDiv.append(butOk);
                                butDiv.append(butCancel);
                                panel.append(butDiv);
                                butCancel.on('click', function () {
                                    $(panel.find('[name="scriptCode"]')[0]).val(businessObject.extensionElements.values[x].code);
                                    $(panel.find('[name="scriptLanguage"]>option[value="' + businessObject.extensionElements.values[x].$type.substring(4) + '"]')[0]).prop('selected', true);
                                });
                                butOk.on('click', function () {
                                    var code = $(panel.find('[name="scriptCode"]')[0]).val();
                                    var lang = $(panel.find('[name="scriptLanguage"]>option:selected')[0]).val();
                                    if (index == null) {
                                        businessObject.extensionElements.values.push({ $type: 'exts:' + lang, id: businessObject.extensionElements.id+'_'+lang });
                                        index = businessObject.extensionElements.values.length - 1;
                                    }
                                    businessObject.extensionElements.values[index].code = code;
                                });
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
                            if (businessObject.extensionElements != undefined) {
                                for (var x = 0; x < businessObject.extensionElements.values.length; x++) {
                                    if (businessObject.extensionElements.values[x].$type == "exts:ConditionSet") {
                                        var cont = $('<div name="condition_container"></div>')
                                        var subDiv = $('<div></div>');
                                        cont.append(subDiv);
                                        dv.append(cont);
                                        var frm = null;
                                        for (var prop in businessObject.extensionElements.values[x]) {
                                            if (renderer[prop] != undefined) {
                                                frm = renderer[prop](businessObject.extensionElements.values[x][prop], options.readonly);
                                                break;
                                            }
                                        }
                                        subDiv.append(frm);
                                        if (!options.readonly) {
                                            var dv = $('<div></div>');
                                            cont.append(dv);
                                            var butOkay = $('<button>Okay</button>');
                                            butOkay.on('click', function (event) {
                                                var tmp = null;
                                                if (contentParser[frm.attr('name')]!=undefined){
                                                    tmp = contentParser[frm.attr('name')](frm,businessObject.id,0);
                                                }
                                                ['isEqualCondition', 'greaterThanCondition', 'greaterThanOrEqualCondition', 'lessThanCondition', 'lessThanOrEqualCondition', 'isNull', 'cSharpScript', 'Javascript', 'VBScript', 'orCondition', 'andCondition','containsCondition'].forEach(function (prop) {
                                                    if (businessObject.extensionElements.values[x].get(prop) != undefined) {
                                                        businessObject.extensionElements.values[x].set(prop, undefined);
                                                    }
                                                });
                                                if (tmp != null) {
                                                    businessObject.extensionElements.values[x].set(tmp.$type.substring(5), tmp);
                                                }
                                            });
                                            dv.append(butOkay);
                                            var butCancel = $('<button>Cancel</button>');
                                            butCancel.on('click', function (event) {
                                                subDiv.html('');
                                                var frm = null;
                                                for (var prop in businessObject.extensionElements.values[x]) {
                                                    if (renderer[prop] != undefined) {
                                                        frm = renderer[prop](businessObject.extensionElements.values[x][prop], options.readonly);
                                                        break;
                                                    }
                                                }
                                                subDiv.append(frm);
                                            });
                                            dv.append(butCancel);
                                        }
                                        break;
                                    }
                                }
                            }
                            break;
                    }
                    panel.show();
                    break;
                default:
                    panel.hide();
                    break;
            }
        });
    };
});