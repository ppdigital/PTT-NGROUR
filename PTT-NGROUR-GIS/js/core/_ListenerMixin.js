/**
 * @description 
 * -
 * @class _ListenerMixin
 */
define([
    "dojo/i18n!nls/global",
    "dojo/store/Memory",
    "dojo/topic",
    "dojo/aspect",
    "dojo/_base/array",
    "dojo/_base/lang",
    "dojo/_base/declare",
    "require"
], function (
    nlsGlobal,
    Memory,
    topic,
    aspect,
    array,
    lang,
    declare,
    require
    ) {


    var wgDeclare = declare(/*[DECLARED_CLASS]*/null, {

        _listening: null,

        constructor: function () {
            this._listening = new Memory({ data: [] });
            this.constructor.listening = new Memory({ data: [] });
        },

        listen: function (evtName, callback) {
            //"/gis/menu/click"
            //"/gis/tool/click"
            if (this._listening.get(evtName)) {
                throw new Error(evtName + " is already listen");
            }
            var handler = this.own(topic.subscribe(evtName, callback))[0];
            this.own(aspect.after(handler, "remove", lang.hitch(this, function (_evtName, argReturn) {
                this._listening.remove(_evtName);
                this.constructor.listening.remove(this.id + "_" + evtName);
                return argReturn;
            }, evtName)));
            this._listening.put({
                id: evtName,
                handler: handler
            });
            this.constructor.listening.put({
                id: this.id + "_" + evtName,
                handler: handler
            });
            return handler;
        },
        call: function (evtName, args) {
            topic.publish(evtName, args);
        },
        destroy: function () {
            array.forEach(this._listening.data, lang.hitch(this, function (d) {
                if (d["handler"]) {
                    d["handler"].remove();
                }
            }));
            this._listening = null;
            this.inherited(arguments);
        }
    });

    return wgDeclare;
});