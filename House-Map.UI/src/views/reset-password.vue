<template>
  <el-dialog
    :close-on-press-escape="false"
    :close-on-click-modal="false"
    title="重置密码"
    :visible.sync="visible"
    :width="isMobile ? '100%' : '550px'"
    :append-to-body="true"
    :before-close="close"
  >
    <div>
      <el-form ref="form" :model="form" label-width="0px" :rules="rules">
        <el-form-item prop="password">
          <el-input v-model="form.password" type="password" placeholder="请输入密码"></el-input>
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
        password: undefined,
        securityCode: undefined
      },
      rules: {
        password: [{ required: true, message: "请输入密码", trigger: "blur" }]
      }
    };
  },
  methods: {
    async submit() {
      try {
        const data = await this.$ajax.post(`/v1/account/reset-password`, {
          password: this.form.password,
          securityCode: this.form.securityCode
        });
        this.$message.success("设置成功");
        this.$router.push("/");
      } catch (e) {
        if (e.message) {
          this.$message.error(e.message);
        }
      }
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
    const query = this.$route.query;
    this.form.password = "";
    this.form.securityCode = query.securityCode;
  }
};
</script>