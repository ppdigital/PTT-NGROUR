/**
 * @description 
 * -
 * @class _WidgetBase
 * @constructor
 * @uses _AlertMixin
 * @uses _RequestMixin
 * @uses _GeometryMixin            
 */
define([
    "dojo/Evented",
    "./_AlertMixin",
    "./_RequestMixin",
    "./_ListenerMixin",
    "./_GeometryMixin",

    "dojo/_base/lang",
    "dojo/_base/declare",
    "require"
], function (
    Evented,
    _AlertMixin,
    _RequestMixin,
    _ListenerMixin,
    _GeometryMixin,

    lang,
    declare,
    require
    ) {

    var wgConstant = {};

    var wgDeclare = declare(/*[DECLARED_CLASS]*/[
        Evented,
        _AlertMixin,
        _RequestMixin,
        _ListenerMixin,
        _GeometryMixin
    ], {

    });

    lang.mixin(wgDeclare, wgConstant);

    return wgDeclare;
});
