import Vue from 'vue';

module.exports = function (props = {}) {
  return Vue.extend({
      data() {
          return {
              visible: true
          }
      },
      methods: {
          close(data) {

          },
          cancel(data) {

          }
      },
      render() {
          return (
              '<el-dialog :visible.sync="visible" onClose={this.close} onCancel={this.cancel} {...props} />'
          )
      }
  })
};