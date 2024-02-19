export default (self:any) => {
  return new Promise(async (resolve, reject) => {
    const u = localStorage.getItem('u');
    if (!u) {
      self.$store.dispatch('UserLogout');
    } else {
      try {
        const userId = JSON.parse(u).id;
        const data = await self.$ajax.get(`v1/users/${userId}`);
        self.$store.dispatch('UpdateUserInfo', data.data);
        resolve()
      } catch (e) {
        self.$store.dispatch('UserLogout');
        reject();
      }
    }
  })
}