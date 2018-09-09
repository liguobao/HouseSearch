<template>
  <div>
    <div class="tips">
      <span>厦门租房: </span>
      <a href="https://www.douban.com/group/XMhouse/" target="_blank" class="highlight-name">www.douban.com/group/XMhouse</a>
    </div>
    <el-form ref="form" :model="form" label-width="0px" class="form" :rules="rules">
      <el-form-item label="" prop="cityName">
        <el-input v-model="form.cityName" placeholder="（如：厦门）"></el-input>
      </el-form-item>
      <el-form-item label="" prop="groupId">
        <el-input v-model="form.groupId" placeholder="（豆瓣小组GroupID,如：XMhouse）"></el-input>
      </el-form-item>
      <el-form-item label="">
        <div class="text-center btn-wrap">
          <el-button type="primary" @click="add" :loading="loading" class="">添加此豆瓣小组数据</el-button>
        </div>
      </el-form-item>
    </el-form>
  </div>
</template>
<style lang="scss" scoped>
  .btn-wrap {
    margin-top: 15px
  }
  .tips{
    margin-bottom: 20px;
    font-size: 18px;
    span{

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
          cityName: [
            {required: true, message: '请输入名称', trigger: 'change'},
          ],
          groupId: [
            {required: true, message: '请输入豆瓣小组GroupID', trigger: 'change'},
          ]
        },
        loading: false
      }
    },
    methods: {
      async add() {
        try {
          await this.$refs.form.validate();
          this.loading = true;
          await this.$ajax.post(`/houses/douban`,{
            ...this.form
          });
          this.$message.success('添加成功');
          this.form = {};
          setTimeout(() => {
            this.$refs.form.clearValidate()
          },0);
          this.loading = false;
        } catch (e) {
          this.loading = false;
        }
      }
    }
  }
</script>