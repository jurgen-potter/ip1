import { createApp } from 'vue'
import App from './App.vue'

import './index.scss';
import './styles/admin/editQuestionnaire.scss';
import './styles/recruitment/index.scss';


import './ts/recommendation/endVote.ts';
import './ts/recommendation/vote.ts';
import './ts/recommendation/votersModalLoader';
import './ts/dashboard/timeline.ts';
import './ts/recruitment/index.ts';
import './ts/memberRegister/registerMember.ts';
import './ts/admin/editQuestionnaire.ts';
import './ts/account/login.ts';

createApp(App).mount('#app') 