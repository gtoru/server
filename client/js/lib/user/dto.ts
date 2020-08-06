import { TestSessionInfo, TestSessionResult, UserStats } from "./models";

export class NewSessionRequest {
    public quizId: string;
}

export class TestSessionInfoResponse {
    public testSessionId: string;
    public tasks: QuizTaskResponse[];

    public static toModel(
        newSession: TestSessionInfoResponse
    ): TestSessionInfo {
        return {
            tasks: newSession.tasks,
            testSessionId: newSession.testSessionId,
        };
    }
}

export class QuizTaskResponse {
    public question: string;
    public answers: string[];
}

export class TestSessionResultResponse {
    public testSessionId: string;
    public result: number;

    public static toModel(
        result: TestSessionResultResponse
    ): TestSessionResult {
        return {
            result: result.result,
            testSessionId: result.testSessionId,
        };
    }
}

export class TestSessionResultsResponse {
    public results: TestSessionResultResponse[];

    public static toModel(
        results: TestSessionResultsResponse
    ): TestSessionResult[] {
        return results.results.map((r) => TestSessionResultResponse.toModel(r));
    }
}

export class AnswerDto {
    public taskNumber: number;
    public answer: string;
}

export class AddAnswersRequest {
    public answers: AnswerDto[];
}

export class UserCountResponse {
    public userCount: number;
}

export class UserStatsResponse {
    public userStats: UserStatsDto[];
}

export class UserStatsDto {
    public email: string;
    public result: number;

    public static toModel(dto: UserStatsDto): UserStats {
        return {
            result: dto.result,
            userMail: dto.email,
        };
    }
}
