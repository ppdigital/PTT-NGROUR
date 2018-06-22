define([
	"dojo/on",
	"dojo/dom-construct",
	"dojo/dom-class",
	"dojo/_base/lang",
	"dojo/_base/declare",
	"require"
], function (
	on,
	domConstruct,
	domClass,
	lang,
	declare,
	require
	) {

    var wgConstant = {};

    var wgDeclare = declare(null, {

        notificationContainer: null,

        showMessage: function (message, level, timeout, noAnimation) {
            this.clearMessages();

            var notification = domConstruct.create("div", { className: "snack-bar-pane snack-bar-pane-hide" }, this.notificationContainer);

            if (noAnimation)
                domClass.add(notification, "snack-bar-pane-no-animation");

            if (level == "loading") {
                timeout = 0;
                domClass.add(notification, "snack-bar-pane-loading");

                var notificationMessage = domConstruct.create("div", { className: "snack-bar-message" }, notification);
            }
            else {
                var closeButton = domConstruct.create("div", { className: "snack-bar-close-button" }, notification);
                var notificationIcon = domConstruct.create("div", { className: "snack-bar-icon" }, notification);

                if (level == "info")
                    domClass.add(notificationIcon, "snack-bar-icon-info");
                else if (level == "warning")
                    domClass.add(notificationIcon, "snack-bar-icon-warning");
                else if (level == "success")
                    domClass.add(notificationIcon, "snack-bar-icon-success");
                else
                    domClass.add(notificationIcon, "snack-bar-icon-info");

                var notificationMessage = domConstruct.create("div", { className: "snack-bar-message" }, notification);
                notificationMessage.innerHTML = message;

                //set close event handler.
                this.own(
					on(closeButton, "click", function () {
					    domClass.add(notification, "snack-bar-pane-hide");

					    setTimeout(function () {
					        domConstruct.destroy(notification);
					    }, 1000);
					})
				);
            }

            //show notifiation.
            if (!noAnimation) {
                setTimeout(function () {
                    domClass.remove(notification, "snack-bar-pane-hide");
                }, 100);
            }
            else
                domClass.remove(notification, "snack-bar-pane-hide");

            //set timeout.
            if (typeof timeout !== "number" || isNaN(timeout))
                timeout = 0;

            if (timeout > 20)
                timeout = 20;
            if (timeout <= 0)
                timeout = 0;

            if (timeout != 0)
                setTimeout(function () {
                    domClass.add(notification, "snack-bar-pane-hide");

                    setTimeout(function () {
                        domConstruct.destroy(notification);
                    }, 1000);

                }, timeout * 1000);
        },

        clearMessages: function () {
            domConstruct.empty(this.notificationContainer);
        },

        postCreate: function () {
            this.notificationContainer = domConstruct.create("div", { className: "snack-bar-container" }, this.domNode);
        }
    });

    return wgDeclare;
});