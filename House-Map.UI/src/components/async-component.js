import Vue from 'vue'

export default function (com, props, cb) {
    return new Promise((resolve, reject) => {
        const template = Vue.extend({
            components: {
                com
            },
            methods: {
                close(data) {
                    resolve(data)
                },
                cancel(data) {
                    reject(data)
                }
            },
            render() {
                return (
                    <com onClose={this.close} onCancel={this.cancel} {...props}  />
                )
            }
        });
        cb && cb(template)
        // const component = new template().$mount();
        // document.querySelector(el).appendChild(component.$el)
    })
}