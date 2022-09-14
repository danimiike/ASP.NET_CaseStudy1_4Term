<template>
<img :src="`/store.png`" class="store-image" />
    <div>
        <div class="heading">Login</div>
        <div class="status">{{ state.status }}</div>
        <div class="p-fluid">
            <div class="p-field">
                <label class="p-field-label" for="email">Email</label>
                <InputText
                    type="text"
                    placeholder="enter email address"
                    id="email"
                    v-model="state.email"
                    :class="{ 'p-invalid': state.validationErrors.email }"
                />
                <small v-show="state.validationErrors.email" class="p-error"
                    >valid format is required.</small
                >
            </div>
            <div class="p-field">
                <label for="password" class="p-field-label">Password</label>
                <span class="p-input-icon-right">
                    <InputText
                        placeholder="enter password"
                        id="password"
                        :type="state.visibility"
                        v-model="state.password"
                        :class="{
                            'p-invalid': state.validationErrors.password,
                        }"
                    />
                    <i
                        v-if="state.visibility === 'password'"
                        class="pi pi-eye"
                        @click="showPassword"
                    />
                    <i
                        v-if="state.visibility === 'text'"
                        class="pi pi-eye-slash"
                        @click="hidePassword"
                    />
                </span>
                <small v-show="state.validationErrors.password" class="p-error"
                    >Password is required.</small
                >
            </div>
            <Button
                label="Login"
                @click="login"
                class="p-button-outlined"
                style="margin: 5vh; width: 25vw"
            />
        </div>
    </div>
</template>
<script>
import { poster } from "../util/apiutil";
import { reactive } from "vue";
import { useRouter, useRoute } from "vue-router";
export default {
    setup() {
        const router = useRouter();
        const route = useRoute();
        let state = reactive({
            email: "",
            password: "",
            status: "",
            visibility: "password",
            validationErrors: [],
        });
        const login = async () => {
            await sessionStorage.removeItem("customer");
            try {
                !state.email.trim()
                    ? (state.validationErrors["email"] = true)
                    : delete state.validationErrors["email"];
                !state.password.trim()
                    ? (state.validationErrors["password"] = true)
                    : delete state.validationErrors["password"];
                if (Object.keys(state.validationErrors).length === 0) {
                    state.status = "logging into server";
                    let userHelper = {
                        firstname: "",
                        lastname: "",
                        email: state.email,
                        password: state.password,
                    };
                    let payload = await poster("login", userHelper); // in util
                    if (payload.token.indexOf("failed") > 0) {
                        state.status = payload.token;
                    } else {
                        await sessionStorage.setItem(
                            "customer",
                            JSON.stringify(payload)
                        );
                        route.params.nextUrl
                            ? router.push({ name: route.params.nextUrl })
                            : router.push({ name: "Home" });
                    }
                }
            } catch (err) {
                console.log(`error ${err}`);
                state.status = `Error has occured: ${err}`;
            }
        };
        const showPassword = () => {
            state.visibility = "text";
        };
        const hidePassword = () => {
            state.visibility = "password";
        };
        return {
            login,
            state,
            showPassword,
            hidePassword,
        };
    },
};
</script>