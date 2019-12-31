({
    // appDir: './',
    baseUrl: './',
    // dir: './dist',
    // modules: [
    //     {
    //         name: 'home'
    //     }
    // ],
    fileExclusionRegExp: /^(r|build)\.js$/,
    optimizeCss: 'standard',
    removeCombined: true,
    paths: {
        jquery: 'lib/jquery-1.11.3.min',
        es5: 'lib/es5',
        "jquery.range": 'lib/jquery.range',
        AMUI: 'lib/amazeui.2.7.1.min',
        addToolbar: 'lib/addToolbar',
    },

    shim: {
        "jquery.range": {
            deps: ["jquery"]
        },
        AMUI: {
            deps: ["jquery"]
        },
        addToolbar: {
            deps: ["jquery"]
        }
    },
    name: "home",
    out: "housecommom.js"
})