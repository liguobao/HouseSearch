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
        jquery: 'jquery-1.11.3.min',
        es5: 'es5',
        "jquery.range": 'jquery.range',
        AMUI: 'amazeui.2.7.1.min',
        addToolbar: 'addToolbar',
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
    out: "main-built.js"
})