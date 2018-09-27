<template>
  <el-dialog
      ref="login"
      title="地图搜租房"
      :width="isMobile ? '100%' : '700px'"
      center
      :visible="loginVisible"
      :append-to-body="appendToBody	"
      :before-close="() => {close('loginVisible')}"
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
      loginVisible: {
        default: true
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
    methods: {
      close(){
        this.$emit('cancel',false);
      },
      submit(key,val) {
        this.$emit('close',val);
      }
    }
  }
</script>