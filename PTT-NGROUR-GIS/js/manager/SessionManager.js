define([
    "core/_ClassBase",
    "core/_WidgetBase",

    "dojo/on",
    "dojo/_base/lang",
    "dojo/_base/declare",
    "require"
], function (
    _ClassBase,
    _WidgetBase,

    on,
    lang,
    declare,
    require
    ) {

    var instance = null;
    var wgConstant = {};

    var wgDeclare = declare(/*[DECLARED_CLASS]*/[
        _ClassBase
    ], {

        timeout: 30,
        interval: 10,

        _session: null,
        _init: function (loginResult) {
            this._session = loginResult.session;
        },
        getSession: function () {
            return this._session;
        },
        start: function () {
            this.reset();
            this._intervalHandler = setInterval(lang.hitch(this, function () {
                var dataStore = this.reqUrl("ashx/proxy.ashx?ping");
                dataStore.useProxy = false;
                dataStore.query();
            }), this.interval * 60 * 1000);
            if (this.timeout > 0) {
                require(["dojox/timing"], lang.hitch(this, function (timing) {
                    this._timer = new timing.Timer(1000);
                    this._timer.onTick = lang.hitch(this, function () {
                        this._counter = this._counter || 0;
                        this._counter++;
                        if (this._counter >= (this.timeout * 60)) {
                            this.end();
                        }
                    });
                    this._timer.start();
                    if (this._handler) {
                        this._handler.reomve();
                        this._handler = null;
                    }
                    this._handler = on(document.body, "mousemove", lang.hitch(this, "resetCounter"));
                }));
            }
            this.emit("session-start", {
                data: this._session,
                target: this
            });
        },
        resetCounter: function () {
            this._counter = 0;
        },
        end: function () {
            this.reset();
            this.emit("session-end", {
                data: this._session,
                target: this
            });
        },
        reset: function () {
            this._counter = 0;
            if (this._handler) {
                this._handler.remove();
                this._handler = null;
            }
            if (this._timer) {
                this._timer.stop();
                this._timer.onTick = function () { };
                this._timer = null;
            }

            if (this._intervalHandler) {
                clearInterval(this._intervalHandler);
                this._intervalHandler = null;
            }
            this.emit("session-reset", {
                data: this._session,
                target: this
            });
        }
    });

    wgConstant.getInstance = function (loginResult) {
        if (instance === null) {
            instance = new wgDeclare();
            window._sessionManager = instance;
        }
        if (loginResult) {
            instance._init(loginResult);
        }
        return instance;
    }

    lang.mixin(wgDeclare, wgConstant);


    return wgDeclare;
});
