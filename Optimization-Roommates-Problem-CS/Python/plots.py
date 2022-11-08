import numpy as np
import matplotlib.pyplot as plt
import pandas as pd

def normalize(value, minValue, maxValue):
    return (value - minValue) / (maxValue - minValue);

def make_plot(exp_number, deon, exp_data, k):
    if exp_number == 2 or exp_number == 3:
        obj_str = ""
    elif deon == 1:
        obj_str = "Deon"
    else:
        obj_str = "Util"

    if exp_data == "Value" or exp_data == "Time":
        branch_and_bound = 1
    else:
        branch_and_bound = 0

    #k_str = r"\left\lfloor 1+\tfrac{n}{4}\right\rfloor"

    exp_file = "Experiment" + str(exp_number) + obj_str + k + exp_data


    data = pd.read_csv("Experiments/" + exp_file + ".csv")
    data = pd.DataFrame.to_numpy(data)

    #remove zero rows
    data = data[~np.all(data == -1, axis=1)]

    print("Experiment " + str(exp_number) + ", " + exp_data + ", " + k)

    #first column contains all values of n
    ns = data[:,0]
    if exp_data == "Value" and (exp_number == 1 or exp_number == 2):
        for i in range(len(data)):
            n = data[i][0]
            kValue = [2, n // 4, n // 2][int(k[1])] if exp_number == 1 else n // 5
            maxWeight = n if exp_number == 1 else 4
            minValue = (n / kValue) - 1 if exp_number == 1 else 0
            maxValue = maxWeight * ((n / kValue) - 1)
            if deon != 1:
                minValue *= n
                maxValue *= n
            # counter-act subtracting minValue from std in normalize
            for j in range(2, len(data[i]), 2):
                if data[i][j-1] != -1:
                    data[i][j] += minValue
            for j in range(1, len(data[i])):
                if data[i][j] != -1:
                    data[i][j] = normalize(data[i][j], minValue, maxValue)
            #data[i][2::2] += minValue
            #data[i][1:] = normalize(data[i][1:], minValue, maxValue)

    #second column contains simple hill climbing, third the standard deviation for simple hill climbing
    simple_hc = data[:,1]
    simple_hc_std = data[:,2]

    steepest_hc = data[:,3]
    steepest_hc_std = data[:,4]

    fc_hc = data[:,5]
    fc_hc_std = data[:,6]

    stoc_hc = data[:,7]
    stoc_hc_std = data[:,8]

    rr_hc = data[:,9]
    rr_hc_std = data[:,10]

    sa = data[:,11]
    sa_std = data[:,12]

    if exp_data == "Value" and exp_number != 3:
        plt.clf()
        plt.scatter(ns, sa_std, c='b', label="Simulated Annealing")
        plt.scatter(ns, simple_hc_std, c='r', label="Simple HC")
        plt.scatter(ns, steepest_hc_std, c='g', label="Steepest Ascent HC")
        plt.scatter(ns, fc_hc_std, c='c', label="First-Choice HC")
        plt.scatter(ns, stoc_hc_std, c='y', label="Stochastic HC")
        plt.scatter(ns, rr_hc_std, c='k', label="Random Restart HC")
        plt.xlabel(r"n")
        plt.ylabel("Standard deviation (normalized objective value)")
        handles, labels = plt.gca().get_legend_handles_labels()
        by_label = dict(zip(labels, handles))
        plt.legend(by_label.values(), by_label.keys())
        plt.savefig("Plots/" + exp_file + "Std")
        plt.clf()

    if branch_and_bound:
        bab = data[:,13]

    plt.plot(ns, simple_hc, 'r', label="Simple HC", alpha=0.8)

    plt.plot(ns, steepest_hc, 'g', label="Steepest Ascent HC", alpha=0.8)
    
    plt.plot(ns, fc_hc, 'c', label="First-Choice HC", alpha=0.8)

    plt.plot(ns, stoc_hc, 'y', label="Stochastic HC", alpha=0.8)

    if exp_number != 3:
        plt.plot(ns, rr_hc, 'k', label="Random Restart HC", alpha=0.8)

    plt.plot(ns, sa, 'b', label="Simulated Annealing", alpha=0.8)

    if exp_data != "Value":
        if not ((exp_number == 2 and exp_data == "NeighborsPerSwap") or (exp_number == 2 and exp_data == "Time")):
            plt.plot(ns, simple_hc - simple_hc_std, 'r--')
        plt.plot(ns, simple_hc + simple_hc_std, 'r--')
        if not ((exp_number == 2 and exp_data == "Swaps") or (exp_number == 2 and exp_data == "Time")):
            plt.plot(ns, steepest_hc - steepest_hc_std, 'g--')
        plt.plot(ns, steepest_hc + steepest_hc_std, 'g--')
        if not (exp_number == 2 and exp_data == "NeighborsPerSwap"):
            plt.plot(ns, fc_hc - fc_hc_std, 'c--')
        plt.plot(ns, fc_hc + fc_hc_std, 'c--')
        if not(exp_number == 2 and exp_data == "NeighborsPerSwap"):
            plt.plot(ns, stoc_hc - stoc_hc_std, 'y--')
        plt.plot(ns, stoc_hc + stoc_hc_std, 'y--')
        if not (((exp_number == 1 and deon != 1 and k == "k0") or exp_number == 2) and exp_data == "Swaps"):
            plt.plot(ns, sa - sa_std, 'b--')
        plt.plot(ns, sa + sa_std, 'b--')

        if exp_number != 3:
            plt.plot(ns, rr_hc - rr_hc_std, 'k--')
            plt.plot(ns, rr_hc + rr_hc_std, 'k--')

    if branch_and_bound and exp_number != 3 and not k == "k2":
        plt.plot(ns[bab != -1], bab[bab != -1], 'm', label="Branch and Bound")

    if exp_number == 3 and exp_data == "Value":
        plt.clf()
        #optimal solution has value n
        plt.plot(ns, ns, 'm', label="Optimal solution")

        plt.plot(ns, simple_hc, 'r', label="All hill climbers")

        plt.plot(ns, sa, 'b', label="Simulated Annealing")

        plt.xlabel(r"n")
        plt.ylabel("Objective value")
        plt.yscale('log')

        handles, labels = plt.gca().get_legend_handles_labels()
        by_label = dict(zip(labels, handles))
        plt.legend(by_label.values(), by_label.keys())
        plt.savefig("Plots/" + exp_file)
        plt.clf()
        return

    plt.xlabel(r"n")
    if exp_data == "Value":
        plt.ylabel("Normalized objective value")
    elif exp_data == "Time":
        plt.ylabel("Time (seconds)")
    elif exp_data == "Swaps":
        plt.ylabel("Number of swaps performed")
    elif exp_data == "NeighborsPerSwap":
        plt.ylabel("Number of neighbors considered per swap")
    elif exp_data == "SwapDelta":
        plt.ylabel("Average difference in objective value per swap")

    if exp_data == "Time":
        plt.xscale('log')

    if exp_data != "SwapDelta" and exp_data != "Value": #and exp_data != "NeighborsPerSwap":
        plt.yscale('log')
        
    handles, labels = plt.gca().get_legend_handles_labels()
    by_label = dict(zip(labels, handles))
    plt.legend(by_label.values(), by_label.keys())
    plt.savefig("Plots/" + exp_file)
    plt.clf()

for exp_number in [1, 2, 3]:
    for exp_data in ["Time", "Value", "NeighborsPerSwap", "SwapDelta", "Swaps"]:
        
        if exp_number == 1:
            for k in ["k0", "k1", "k2"]:
                for deon in [0, 1]:
                    make_plot(exp_number, deon, exp_data, k)
        
        elif exp_number == 2:
            make_plot(exp_number, 1, exp_data, "k0")

        elif exp_number == 3:
            make_plot(exp_number, 1, exp_data, "k0")