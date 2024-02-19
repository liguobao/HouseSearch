import Vue, {VNode, CreateElement} from 'vue'

export default function (com: any, props: any, cb: Function) {
  return new Promise((resolve, reject) => {
    const template = Vue.extend({
      components: {
        com
      },
      methods: {
        close(data: any) {
          resolve(data)
        },
        cancel(data: any) {
          reject(data)
        }
      },
      render(h: CreateElement): VNode {
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