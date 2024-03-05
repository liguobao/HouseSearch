<template>
  <div>
    <div class="tips">
      <span>发邮件给我~</span>
    </div>
    <el-form ref="form" :model="form" label-width="0px" class="form" :rules="rules">
      <el-form-item label="" prop="city">
        <el-input v-model="form.city" placeholder="城市，如厦门"></el-input>
      </el-form-item>
      <el-form-item label="" prop="source">
        <el-input v-model="form.source" placeholder="来源链接（豆瓣厦门租房小组、小红书、链家等~）"></el-input>
      </el-form-item>
      <el-form-item label="" prop="feedbackUser">
        <el-input v-model="form.feedbackUser" placeholder="联系邮箱 or 微信~"></el-input>
      </el-form-item>
      <el-form-item label="">
        <div class="text-center btn-wrap">
          <el-button type="primary" @click="add" :loading="loading" class="">发送</el-button>
        </div>
      </el-form-item>
    </el-form>
  </div>
</template>
<style lang="scss" scoped>
.btn-wrap {
  margin-top: 15px;
}
.tips {
  margin-bottom: 20px;
  font-size: 18px;
  span {
  }
}
.highlight-name {
  color: #0e90d2;
  transition: all 0.3s;
  &:hover {
    color: #095f8a;
  }
}
</style>
<script>
export default {
  data() {
    return {
      form: {},
      rules: {
        city: [{ required: true, message: "请输入名称", trigger: "change" }],
        groupId: [
          {
            required: true,
            message: "请输入来源",
            trigger: "change"
          }
        ]
      },
      loading: false
    };
  },
  methods: {
    async add() {
      try {
        await this.$refs.form.validate();
        this.loading = true;
        await this.$ajax.post(`v2/cities/source-feedback`, {
          ...this.form
        });
        if (gtag) {
          gtag("event", "提交租房数据源", {
            event_category: this.form.city
          });
        }
        this.$message.success("添加成功");
        this.form = {};
        setTimeout(() => {
          this.$refs.form.clearValidate();
        }, 0);
        this.loading = false;
      } catch (e) {
        this.loading = false;
      }
    }
  }
};
</script>