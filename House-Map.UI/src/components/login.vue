<template>
  <div class="login">
    <div class="words text-center">
      <template v-if="type === 'login'">
        登录后查看个人收藏房源
      </template>
      <template v-else-if="type === 'register'">
        注册后可以收藏房源信息
      </template>
      <br>
      房子是租来的,而生活不是。
    </div>
    <el-form ref="form" :model="form" label-width="100px" :rules="rules">
      <el-collapse-transition>
        <div v-if="type === 'login'">
          <el-form-item label="账号" prop="userName">
            <el-input v-model.trim="form.userName" placeholder="邮箱/用户名" :maxlength="100"></el-input>
          </el-form-item>
          <el-form-item label="密码" prop="password">
            <el-input v-model.trim="form.password" placeholder="密码" type="password" :maxlength="30"></el-input>
          </el-form-item>
        </div>
      </el-collapse-transition>
      <el-collapse-transition>
        <div v-if="type === 'register'">
          <el-form-item label="昵称" prop="userName">
            <el-input v-model.trim="form.userName" placeholder="昵称" :maxlength="14"></el-input>
          </el-form-item>
          <el-form-item label="邮箱" prop="email">
            <el-input v-model.trim="form.email" placeholder="邮箱" :maxlength="100"></el-input>
          </el-form-item>
          <el-form-item label="密码" prop="password">
            <el-input v-model.trim="form.password" placeholder="密码" type="password" :maxlength="30"></el-input>
          </el-form-item>
          <el-form-item label="" prop="checked">
            <el-checkbox v-model="form.checked"></el-checkbox>
            <span class="register-declaration" @click="terms = !terms">《用户协议》</span>
            <el-collapse-transition>
              <div v-show="terms" class="terms">
                <div>
                  当您申请用户时，表示您已经同意遵守本规章。<br>
                  欢迎您加入本站点参与交流和讨论，本站点为社区，为维护网上公共秩序和社会稳定，请您自觉遵守以下条款：<br>
                  <br>
                  一、不得利用本站危害国家安全、泄露国家秘密，不得侵犯国家社会集体的和公民的合法权益，不得利用本站制作、复制和传播下列信息：<br>
                  （一）煽动抗拒、破坏宪法和法律、行政法规实施的；<br>
                  （二）煽动颠覆国家政权，推翻社会主义制度的；<br>
                  （三）煽动分裂国家、破坏国家统一的；<br>
                  （四）煽动民族仇恨、民族歧视，破坏民族团结的；<br>
                  （五）捏造或者歪曲事实，散布谣言，扰乱社会秩序的；<br>
                  （六）宣扬封建迷信、淫秽、色情、赌博、暴力、凶杀、恐怖、教唆犯罪的；<br>
                  （七）公然侮辱他人或者捏造事实诽谤他人的，或者进行其他恶意攻击的；<br>
                  （八）损害国家机关信誉的；<br>
                  （九）其他违反宪法和法律行政法规的；<br>
                  （十）进行商业广告行为的。<br>
                  <br>
                  二、互相尊重，对自己的言论和行为负责。<br>
                  三、禁止在申请用户时使用相关本站的词汇，或是带有侮辱、毁谤、造谣类的或是有其含义的各种语言进行注册用户，否则我们会将其删除。<br>
                  四、禁止以任何方式对本站进行各种破坏行为。<br>
                  五、如果您有违反国家相关法律法规的行为，本站概不负责，您的登录信息均被记录无疑，必要时，我们会向相关的国家管理部门提供此类信息。
                </div>
              </div>
            </el-collapse-transition>
          </el-form-item>
        </div>
      </el-collapse-transition>
      <el-form-item label="" label-width="0px">
        <div class="btn-wrap text-center">
          <el-button type="primary" @click="submit" :loading="loading" class="submit">{{type === 'register' ? '注册' :
            '登录'}}
          </el-button>
        </div>
      </el-form-item>
    </el-form>
    <div class="text-center">
      <template v-if="type === 'register'">
        已有帐号？
        <el-button type="text" @click="type = 'login'">登录</el-button>
      </template>
      <template v-else-if="type === 'login'">
        没有帐号？
        <el-button type="text" @click="type = 'register'">注册</el-button>
      </template>
    </div>
  </div>
</template>
<style scoped lang="scss">
  .words {
    margin-bottom: 20px;
  }

  .btn-wrap {
    margin-top: 10px;
  }

  .submit {
    width: 100%;
  }

  .register-declaration {
    color: #409EFF;
    cursor: pointer;
  }

  .terms {
    background: #f5f5f5;
    padding: 20px;
    > div {
      max-height: 200px;
      overflow: auto;
      word-break: break-all;
    }
  }
</style>
<script>
  export default {
    props: {
      loginType: {
        default: ''
      }
    },
    watch: {
      loginType(n) {
        if(n) {
          this.type = n;
        }
      }
    },
    data() {
      return {
        form: {
          checked: true,
        },
        type: this.loginType ? this.loginType : 'login',
        terms: false,
        rules: (() => {
          const register = (rule, value, callback) => {
            if (!value) {
              return callback(new Error())
            }
            callback()
          };
          return {
            userName: [
              {required: true, message: this.type === 'login' ? '请输入账号' : '请输入昵称', trigger: 'change'},
              {min: 2,  message: '至少2个字符', trigger: 'change'}
            ],
            email: [
              {required: true, message: '请输入邮箱地址', trigger: 'blur'},
              {type: 'email', message: '请输入正确的邮箱地址', trigger: ['blur', 'change']}
            ],
            password: [
              {required: true, message: '请输入密码', trigger: 'change'},
            ],
            checked: [
              {required: true, message: '请同意用户协议', trigger: 'change', validator: register},
            ]
          }
        })(),
        loading: false
      }
    },
    methods: {
      async submit() {
        await this.$refs.form.validate();
        if (this.type === 'register') {
          this.register()
        }else {
          this.login();
        }
      },
      async login() {
        try {
          this.loading = true;
          const params = Object.assign({}, this.form);
          delete params.checked;
          const data = await this.$ajax.post('/account', params);
          this.$store.dispatch('UserLogin', data);
          this.$message.success(data.message ? data.message : '登录成功');
          this.loading = false;
          this.$emit('close', 'loginVisible', false)
        } catch (e) {
          if (e.message) {
            this.$message.success(e.message);
          }
          this.loading = false;
          throw e
        }
      },
      async register() {
        try {
          this.loading = true;
          const params = Object.assign({}, this.form);
          delete params.checked;
          const data = await this.$ajax.post('/account/register', params);
          this.$store.dispatch('UserLogin', data);
          this.$message.success(data.message ? data.message : '注册成功');
          this.loading = false;
          this.$emit('close', 'loginVisible', false)
        } catch (e) {
          if (e.message) {
            this.$message.success(e.message);
          }
          this.loading = false;
          throw e
        }
      }
    }
  }
</script>