<template>
    <el-dialog
            title="登录"
            :width="isMobile ? '100%' : '900px'"
            center
            :visible="visible"
            :append-to-body="appendToBody"
            :before-close="cancel"
    >
        <my-login @close="submit"></my-login>
    </el-dialog>
</template>
<style lang="scss" scoped>

</style>
<script>
    import login from './login'
    export default {
        components:{
            myLogin:login
        },
        props: {
            appendToBody: {
                default: false
            },
            isMobile: {
                default: false
            }
        },
        data() {
            return {
                visible: true
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
        }
    }
</script>