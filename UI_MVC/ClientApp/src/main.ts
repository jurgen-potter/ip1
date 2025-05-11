import { createApp } from 'vue'
import App from './App.vue'

import './index.scss';
import './styles/site.scss';
import './styles/admin/editQuestionnaire.scss';
import './styles/panel/dashboard.scss';
import './styles/recruitment/index.scss';
import './styles/test/components.scss';


import './ts/recommendation/endVote.ts';
import './ts/recommendation/vote.ts';
import './ts/dashboard/timeline.ts';
import './ts/recruitment/index.ts';
import './ts/memberRegister/registerMember.ts';
import './ts/admin/editQuestionnaire.ts';
import './ts/account/login.ts';

createApp(App).mount('#app') 