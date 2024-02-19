<template>
  <el-dialog
    :close-on-press-escape="false"
    :close-on-click-modal="false"
    title="设置邮箱"
    :visible.sync="visible"
    :width="isMobile ? '100%' : '550px'"
    :append-to-body="true"
    :before-close="close"
  >
    <div>
      <el-form ref="form" :model="form" label-width="0px" :rules="rules">
        <el-form-item prop="email">
          <el-input v-model="form.email" placeholder="请输入邮箱"></el-input>
        </el-form-item>
      </el-form>
      <div class="btn-wrap">
        <el-button @click="cancel" size="small">取消</el-button>
        <el-button type="primary" @click="submit" size="small">确定</el-button>
      </div>
    </div>
  </el-dialog>
</template>
<style lang="scss" scoped>
.btn-wrap {
  text-align: right;
}
</style>
<script>
export default {
  props: {
    isMobile: {
      default: false
    }
  },
  data() {
    return {
      visible: true,
      input: "",
      form: {
        email: undefined
      },
      rules: {
        email: [
          { required: true, message: "请输入邮箱地址", trigger: "blur" },
          {
            type: "email",
            message: "请输入正确的邮箱地址",
            trigger: ["blur", "change"]
          }
        ]
      }
    };
  },
  methods: {
    async submit() {
      let user = this.$store.state.userInfo;
      if (!user) {
        this.$message.error("请先登录");
        return;
      }
      await this.$refs.form.validate();
      let userId = user.id;
      const data = await this.$ajax.post(`v1/users/${userId}/email`, {
        email: this.form.email
      });
      this.$store.dispatch("UserLogin", {
        ...data,
        token: this.$store.state.token
      });
      this.$message.success("设置成功");
      this.close();
      this.$emit("close");
    },
    close() {
      this.visible = false;
    },
    cancel() {
      this.close();
      this.$emit("cancel");
    }
  },
  created() {
    let user = this.$store.state.userInfo;
    if (user && user.email) {
      this.form.email = user.email;
    }
  }
};
</script>