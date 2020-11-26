import {observable, computed, action, runInAction} from 'mobx';
import {IUser, IUserFormValues} from '../models/user';
import agent from '../api/agent';
import {RootStore} from './rootStore';
import {history} from '../../index';

export default class UserStore {
    refreshTokenTimeOut: any;
    rootStore: RootStore;

    constructor(rootStore: RootStore) {
        this.rootStore = rootStore;
    }

    @observable user: IUser | null = null;
    @observable loading = false;

    @computed get isLoggedIn() {
        return !!this.user;
    }

    @action login = async (values: IUserFormValues) => {
        try {
            const user = await agent.User.login(values);
            runInAction(() => {
                this.user = user;
            });
            this.rootStore.commonStore.setToken(user.token);
            this.startRefreshTokenTimer(user);
            this.rootStore.modalStore.closeModal();
            history.push('/activities');
        } catch (error) {
            throw error;
        }
    };

    @action register = async (values: IUserFormValues) => {
        try {
            await agent.User.register(values);
            this.rootStore.modalStore.closeModal();
            history.push(`/user/registerSuccess?email=${values.email}`);
        } catch (error) {
            throw error;
        }
    };

    @action refreshToken = async () => {
        this.stopRefreshTokenTimer();
        try {
            const user = await agent.User.refreshToken();
            runInAction(() => {
                this.user = user;
            })
            //whenever we user logs in we going to get the user from somewhere
            //so that whatever the user is doing then we now when the token expires
            //we get a new token so we know when the token expires
            this.rootStore.commonStore.setToken(user.token);
            this.startRefreshTokenTimer(user);
        } catch (e) {
            console.log(e)
        }
    }

    @action getUser = async () => {
        try {
            const user = await agent.User.current();
            runInAction(() => {
                this.user = user;
            });
            this.rootStore.commonStore.setToken(user.token);
            this.startRefreshTokenTimer(user);
        } catch (error) {
            console.log(error);
        }
    };

    @action logout = () => {
        this.rootStore.commonStore.setToken(null);
        this.user = null;
        history.push('/');
    };

    @action fbLogin = async (response: any) => {
        this.loading = true;
        try {
            const user = await agent.User.fbLogin(response.accessToken);
            runInAction(() => {
                this.user = user;
                this.rootStore.commonStore.setToken(user.token);
                this.startRefreshTokenTimer(user);
                this.rootStore.modalStore.closeModal();
                this.loading = false;
            });
            history.push('/activities');
        } catch (error) {
            this.loading = false;
            throw error;
        }
    }

    private startRefreshTokenTimer(user: IUser) {
        //parsing token with atob, splitting based on the period
        const jwtToken = JSON.parse(atob(user.token.split('.')[1]));
        const expires = new Date(jwtToken.exp * 1000);
        const timeOut = expires.getTime() - Date.now() - (60 - 1000);
        console.log(jwtToken)
        console.log(expires)
        console.log(timeOut);
        this.refreshTokenTimeOut = setTimeout(this.refreshToken, timeOut);
    }

    private stopRefreshTokenTimer() {
        clearTimeout(this.refreshTokenTimeOut);
    }
}
