define(["require"], function (require) {
    return {
        menus: [
            //{
            //    label: "Sample",
            //    className: "sample",
            //    isHeader: true,
            //    isDivider: true,
            //    iconClassName: "sample",
            //    iconUrl: require.toUrl("./../resources/images/sample.png"),
            //    events: [
            //        {
            //            action: "click",
            //            name: "open-sample"
            //        }
            //    ],
            //    subMenus: [
            //        {
            //            label: "Sample",
            //            className: "sample",
            //            iconClassName: "sample",
            //            iconUrl: require.toUrl("./../resources/images/sample.png"),
            //            events: [
            //                {
            //                    action: "click",
            //                    name: "open-sample"
            //                }
            //            ],
            //        }
            //    ]
            //},
            {
                iconClassName: "routing",
                label: "Routing",
                events: [
                    {
                        action: "click",
                        name: "open-routing"
                    }
                ]
            }, {
                iconClassName: "search",
                label: "Search",
                events: [
                    {
                        action: "click",
                        name: "open-search"
                    }
                ]
            }, {
                iconClassName: "buffer",
                label: "Buffer",
                events: [
                    {
                        action: "click",
                        name: "open-buffer"
                    }
                ]
            }, {
                iconUrl: require.toUrl("./../resources/images/btn_onmap_more.png"),
                label: "Other",
                subMenus: [{
                    iconUrl: require.toUrl("./../resources/images/btn_onmap_more.png"),
                    label: "Sub-menu A"
                }, {
                    iconUrl: require.toUrl("./../resources/images/btn_onmap_more.png"),
                    label: "Sub-menu B"
                }, {
                    iconUrl: require.toUrl("./../resources/images/btn_onmap_more.png"),
                    label: "Sub-menu C"
                }, {
                    iconUrl: require.toUrl("./../resources/images/btn_onmap_more.png"),
                    label: "Sub-menu D"
                }]
            }, {
                isHeader: true,
                className: "only-large",
                label: "Basemaps",
            }, {
                className: "only-large",
                iconUrl: require.toUrl("./../resources/images/btn_onmap_more.png"),
                label: "Nostra"
            }, {
                className: "only-large",
                iconUrl: require.toUrl("./../resources/images/btn_onmap_more.png"),
                label: "Imagery"
            }, {
                className: "only-large",
                iconUrl: require.toUrl("./../resources/images/btn_onmap_more.png"),
                label: "Street"
            }, {
                isDivider: true,
                className: "only-large"
            }, {
                className: "only-large",
                iconUrl: require.toUrl("./../resources/images/btn_onmap_more.png"),
                label: "Username"
            }, {
                className: "only-large",
                iconUrl: require.toUrl("./../resources/images/btn_onmap_more.png"),
                label: "Logout"
            }
        ],

        onMapItems: [
            //{
            //    title: "Sample",
            //    className: "sample",
            //    iconClassName: "sample",
            //    iconUrl: require.toUrl("./../resources/images/sample.png"),
            //    style: {
            //        position: "absolute",
            //        right: "8px",
            //        bottom: "120px"
            //    },
            //    events: [
            //        {
            //            action: "click",
            //            name: "open-sample"
            //        }
            //    ],
            //    subItemPosition: "left",
            //    subItems: [
            //        {
            //            title: "Sample",
            //            className: "sample",
            //            iconClassName: "sample",
            //            iconUrl: require.toUrl("./../resources/images/sample.png"),
            //            events: [
            //                {
            //                    action: "click",
            //                    name: "open-sample"
            //                }
            //            ]
            //        }
            //    ]
            //},
            {
                title: "TOC",
                iconClassName: "toc",
                buttonClassName: "normal",
                style: {
                    position: "absolute",
                    top: "80px",
                    right: "12px"
                },
                events: [
                    {
                        action: "click",
                        name: "open-toc"
                    }
                ]
            }, {
                title: "Measurement",
                iconClassName: "measurement",
                buttonClassName: "normal",
                style: {
                    position: "absolute",
                    top: "113px",
                    right: "12px"
                },
                events: [
                    {
                        action: "click",
                        name: "open-measurement"
                    }
                ]
            }, {
                title: "GotoXY",
                iconClassName: "gotoxy",
                buttonClassName: "normal",
                style: {
                    position: "absolute",
                    top: "146px",
                    right: "12px"
                },
                events: [
                    {
                        action: "click",
                        name: "open-gotoxy"
                    }
                ]
            }, {
                title: "Print Map",
                iconClassName: "printmap",
                buttonClassName: "normal",
                style: {
                    position: "absolute",
                    top: "179px",
                    right: "12px"
                },
                events: [
                    {
                        action: "click",
                        name: "open-printmap"
                    }
                ]
            }, {
                title: "Basemap",
                iconClassName: "basemap",
                buttonClassName: "normal",
                style: {
                    position: "absolute",
                    top: "212px",
                    right: "12px"
                },
                events: [
                    {
                        action: "click",
                        name: "open-basemap"
                    }
                ]
            }/*, {
                title: "MIS Data",
                iconClassName: "xxx",
                buttonClassName: "normal",
                style: {
                    position: "absolute",
                    top: "245px",
                    right: "12px"
                },
                events: [
                    {
                        action: "click",
                        name: "open-misdata"
                    }
                ]
            }*/, {
                iconClassName: "more",
                style: {
                    position: "absolute",
                    top: "8px",
                    right: "8px"
                },
                subItemPosition: "left",
                subItems: [
                    /*{
                        title: "MIS Data",
                        iconClassName: "xxx",
                        events: [
                            {
                                action: "click",
                                name: "open-misdata"
                            }
                        ]
                    },*/
                    {
                        title: "TOC",
                        iconClassName: "toc",
                        events: [
                            {
                                action: "click",
                                name: "open-toc"
                            }
                        ]
                    },
                    {
                        title: "Measurement",
                        iconClassName: "measurement",
                        events: [
                            {
                                action: "click",
                                name: "open-measurement"
                            }
                        ]
                    },
                    {
                        title: "Goto XY",
                        iconClassName: "gotoxy",
                        events: [
                            {
                                action: "click",
                                name: "open-gotoxy"
                            }
                        ]
                    },
                    {
                        title: "Print Map",
                        iconClassName: "printmap",
                        events: [
                            {
                                action: "click",
                                name: "open-printmap"
                            }
                        ]
                    },
                    {
                        title: "Basemap",
                        iconClassName: "basemap",
                        events: [
                            {
                                action: "click",
                                name: "open-basemap"
                            }
                        ]
                    }
                ]
            }
        ],
        systemSelectionItems: [
            //{
            //    label: "Sample",
            //    title: "Sample",
            //    iconClassName: "sample",
            //    events: [
            //        {
            //            action: "click",
            //            name: "system-sample"
            //        }
            //    ]
            //},
            //{
            //    label: "LOGOUT",
            //    title: "Logout",
            //    iconClassName: "logout",
            //    events: [
            //        {
            //            action: "click",
            //            name: "system-logout"
            //        }
            //    ]
            //}, {
            //    label: "GIS",
            //    title: "GIS",
            //    iconClassName: "gis",
            //    events: [
            //        {
            //            action: "click",
            //            name: "system-gis"
            //        }
            //    ]
            //}, {
            //    label: "UM",
            //    title: "UM",
            //    iconClassName: "um",
            //    events: [
            //        {
            //            action: "click",
            //            name: "system-um"
            //        }
            //    ]
            //}
        ]
    }
});