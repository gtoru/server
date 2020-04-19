import * as axios from "axios";

export class ClientBase {
    protected rest: axios.AxiosInstance;
    public constructor(baseUrl: string) {
        this.rest = axios.default.create({
            baseURL: baseUrl,
            timeout: 30000,
        });
    }
}
