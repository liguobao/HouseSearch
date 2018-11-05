<template>
  <el-dialog
      :title="cityName"
      :width="isMobile ? '100%' : '900px'"
      center
      :visible="visible"
      :append-to-body="appendToBody"
      :before-close="cancel"
  >
    <div>
      sssss
      {{aaaa}}
    </div>
  </el-dialog>
</template>
<style lang="scss" scoped>

</style>
<script>
  export default {
    props: {
      appendToBody: {
        default: false
      },
      isMobile: {
        default: false
      },
      aaaa:{},
      title: {
        default: '全部城市'
      }
    },
    data() {
      return {
        visible: true
      }
    },
    computed:{
      cityName() {
        return this.title
      }
    },
    methods: {
      submit() {
        this.close();
        this.$emit('close');
      },
      close() {
        this.visible = false;
      },
      cancel() {
        this.close();
        this.$emit('cancel', false);
      }
    },
    created() {
      console.log(this.title)
    }
  }
</script>