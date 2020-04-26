import {
    UserClient,
    AuthClient,
    AuthToken,
    Task,
    TaskClient,
    QuizClient,
    UserId,
    QuizId,
    User,
    TestSessionId,
} from "../../../client/js/lib/index";

async function sleep(ms: number): Promise<void> {
    return new Promise((resolve) => setTimeout(resolve, ms));
}

let quizId: QuizId;
let userId: UserId;
let userToken: AuthToken;
let testSessionId: TestSessionId;
const baseUrl = "http://localhost:8080";
const client = new UserClient(baseUrl);

const testUser: User = {
    email: "tester",
    password: "tester",
    personalInfo: {
        address: "",
        birthday: new Date(),
        employer: "",
        name: "",
        occupation: "",
    },
};

const firstTask: Task = {
    answer: "42",
    question: "Meaning of life",
    taskId: "",
    variants: ["Life", "Death", "42"],
    weight: 2,
};

const secondTask: Task = {
    answer: "42",
    question: "Meaning of life",
    taskId: "",
    variants: ["Life", "Death", "42"],
    weight: 5,
};

async function authenticateAdmin(): Promise<AuthToken> {
    const client = new AuthClient(baseUrl);
    const authentication = await client.authenticateAsync("admin", "admin");
    return authentication.responseData;
}

async function init(): Promise<void> {
    const adminToken = await authenticateAdmin();

    const taskClient = new TaskClient(baseUrl);
    const quizClient = new QuizClient(baseUrl);

    const firstTaskId = await taskClient.createTaskAsync(firstTask, adminToken);
    const secondTaskId = await taskClient.createTaskAsync(
        secondTask,
        adminToken
    );

    await sleep(1000);

    const quizCreation = await quizClient.createQuizAsync(
        "TestQuiz",
        [firstTaskId.responseData, secondTaskId.responseData],
        adminToken
    );

    quizId = quizCreation.responseData.quizId;

    await sleep(1000);

    const authClient = new AuthClient(baseUrl);
    await authClient.registerAsync(testUser);

    const tokenGet = await authClient.authenticateAsync(
        testUser.email,
        testUser.password
    );

    userToken = tokenGet.responseData;

    const userIdGet = await authClient.getSessionInfoAsync(userToken);

    userId = userIdGet.responseData.userId;
}

beforeAll(async (done) => {
    await init();
    done();
});

describe("test session", () => {
    it("starts new test session", async () => {
        const sessionStart = await client.startNewSessionAsync(
            userId,
            quizId,
            userToken
        );

        expect(sessionStart.responseCode).toBe(200);
        expect(sessionStart.responseData.tasks).toHaveLength(2);
        expect(sessionStart.responseData.tasks[0].question).toBe(
            firstTask.question
        );
        expect(sessionStart.responseData.tasks[0].answers).toStrictEqual(
            firstTask.variants
        );
        expect(sessionStart.responseData.tasks[1].question).toBe(
            secondTask.question
        );
        expect(sessionStart.responseData.tasks[1].answers).toStrictEqual(
            secondTask.variants
        );

        testSessionId = sessionStart.responseData.testSessionId;
    });

    it("gets current session", async () => {
        const currentSession = await client.getCurrentTestSessionAsync(
            userId,
            userToken
        );

        expect(currentSession.responseCode).toBe(200);
        expect(currentSession.responseData.testSessionId).toBe(testSessionId);
    });

    it("adds answers", async () => {
        const answerAdd = await client.addAnswerAsync(
            userId,
            [{ answer: "42", taskNumber: 1 }],
            userToken
        );

        await sleep(1000);

        expect(answerAdd.responseCode).toBe(200);
    });

    it("ends test session", async () => {
        const sessionEnd = await client.endSessionAsync(userId, userToken);

        expect(sessionEnd.responseCode).toBe(200);
        expect(sessionEnd.responseData.result).toBe(5);
        expect(sessionEnd.responseData.testSessionId).toBe(testSessionId);
    });

    it("gets all results", async () => {
        const testSessionResults = await client.getResultsAsync(
            userId,
            userToken
        );

        expect(testSessionResults.responseCode).toBe(200);
        expect(testSessionResults.responseData).toHaveLength(1);
        expect(testSessionResults.responseData[0].result).toBe(5);
        expect(testSessionResults.responseData[0].testSessionId).toBe(
            testSessionId
        );
    });
});
