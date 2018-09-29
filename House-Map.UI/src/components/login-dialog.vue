<template>
  <el-dialog
      ref="login"
      title="地图搜租房"
      :width="isMobile ? '100%' : '700px'"
      center
      :visible="loginVisible"
      :append-to-body="appendToBody	"
      :before-close="() => {cancel('loginVisible')}"
  >
    <login @close="submit"></login>
  </el-dialog>
</template>
<script>
  import login from './login'
  export default {
    props: {
      appendToBody: {
        default: false
      },
      isMobile: {
        default: false
      }
    },
    computed:{

    },
    components: {
      login
    },
    data() {
      return {
        loginVisible: true
      }
    },
    methods: {
      close() {
        this.loginVisible = false;
      },
      cancel(){
        this.close();
        this.$emit('cancel',false);
      },
      submit(key,val) {
        this.close();
        this.$emit('close',val);
      }
    }
  }
</script>