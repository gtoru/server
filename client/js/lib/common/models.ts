export class Response<TData> {
    public responseCode: number;
    public responseData: TData;
    public errorInfo: ErrorInfo;

    public constructor(
        responseCode: number,
        responseData: TData,
        errorInfo?: ErrorInfo
    ) {
        this.responseCode = responseCode;
        this.responseData = responseData;
        this.errorInfo = errorInfo;
    }
}

export type ErrorInfo = string;
