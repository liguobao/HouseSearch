import Vue, {VNode, CreateElement} from 'vue';

module.exports = function (props = {'visible.sync': true}) {
  return Vue.extend({
    data() {
      return {
        visible: true
      }
    },
    methods: {
      close(data: any) {

      },
      cancel(data: any) {

      }
    },
    render(): VNode {
      return (
        <el-dialog visible={this.visible} onClose={this.close} onCancel={this.cancel} {...props} />
      )
    }
  })
};